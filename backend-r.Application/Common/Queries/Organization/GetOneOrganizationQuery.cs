namespace backend_r.Application.Common.Queries.Organization
{
    public class GetOneOrganizationQuery : IRequest<OneOrganizationVm>
    {
        public int Id { get; set; }
    }

    class GetOneOrganizationHandler : IRequestHandler<GetOneOrganizationQuery, OneOrganizationVm>
    {
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IIdDefinitionRepository _idDefinitionRepo;
        private readonly ILogger<GetOneOrganizationHandler> _logger;

        public GetOneOrganizationHandler(IOrganizationRepository organizationRepository,
            IIdDefinitionRepository idDefinitionRepository, ILogger<GetOneOrganizationHandler> logger)
        {
            _idDefinitionRepo = idDefinitionRepository;
            _logger = logger;
            _organizationRepo = organizationRepository;
        }

        public async Task<OneOrganizationVm> Handle(GetOneOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organization = await _organizationRepo.GetByIdAsync(request.Id);

            if (organization == null)
                return null;

            var parentOrganization = await _organizationRepo.GetByIdAsync(organization.ParentId);

            var staffOrganizations = new List<Vms.StaffOrganization>();

            _logger.LogInformation("------Getting Organization-------");

            var oneOrganization = new OneOrganizationVm
            {
                Id = organization.Id,
                Name = organization.Name,
                ShortName = organization.ShortName,
                Description = organization.Description,
                OrganizationTypeName = organization.OrganizationType.Name,
                ParentId = organization.ParentId,
                ParentName = parentOrganization != null ? parentOrganization.Name : null,
                IdDefinitions = organization.IdDefinitions.Count() > 0 ? organization.IdDefinitions.Select(c =>
                    new IdDefinitionVm
                    {
                        Prefix = c.Prefix,
                        Suffix = c.Suffix,
                        Length = c.Length,
                        PrefSep = c.PrefSep,
                        SuffSep = c.SuffSep,
                        NextValue = c.NextValue
                    }).ToList() : null,
                StaffOrganizations = organization.StaffOrganizations.Count() > 0 ? organization.StaffOrganizations.Select(c =>
                    new Vms.StaffOrganization
                    {
                        IsActive = c.IsActive,
                        IsContactPerson = c.IsContactPerson,
                        Staffs = c.Staff != null ? new List<AllStaffVm>{
                            new AllStaffVm{
                                Id = c.Staff.Id,
                                Title = c.Staff.Title,
                                FirstName = c.Staff.FirstName,
                                MiddleName = c.Staff.MiddleName,
                                LastName = c.Staff.LastName,
                                GenderName = c.Staff.Gender.Name
                            }
                        } : null
                    }).ToList() : null
            };

            return oneOrganization;
        }
    }
}