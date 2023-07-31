namespace backend_r.Application.Common.Queries.Organization
{
    public class GetAllOrganizationQuery : IRequest<IEnumerable<AllOrganizationVm>> {}

    class GetAllOrganizationHandler : IRequestHandler<GetAllOrganizationQuery, IEnumerable<AllOrganizationVm>>
    {
        private readonly IOrganizationRepository _organizationRepo;
        private readonly ILogger<GetAllOrganizationHandler> _logger;

        public GetAllOrganizationHandler(IOrganizationRepository organizationRepository,
            ILogger<GetAllOrganizationHandler> logger)
        {
            _organizationRepo = organizationRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllOrganizationVm>> Handle(GetAllOrganizationQuery request, CancellationToken cancellationToken)
        {
            var organizations = await _organizationRepo.GetAllAsync();
            var allOrganizationVm = new List<AllOrganizationVm>();

            for (int i = 0; i < organizations.Count(); i++)
            {
                var parentOrganization = await _organizationRepo.GetByIdAsync(organizations[i].ParentId);

                allOrganizationVm.Add(new AllOrganizationVm
                    {
                        Id = organizations[i].Id,
                        Name = organizations[i].Name,
                        ShortName = organizations[i].ShortName,
                        ParentId = organizations[i].ParentId,
                        ParentName = parentOrganization != null ? parentOrganization.Name : null,
                        Description = organizations[i].Description,
                        OrganizationTypeName = organizations[i].OrganizationType.Name,
                        IsActive = organizations[i].IsActive
                    });
            }

            _logger.LogInformation("------Getting Organizations--------");

            return allOrganizationVm;
        }
    }
}