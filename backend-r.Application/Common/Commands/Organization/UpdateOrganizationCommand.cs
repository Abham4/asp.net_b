namespace backend_r.Application.Common.Commands.Organization
{
    public class UpdateOrganizationCommand : IRequest<string>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public int OrganizationTypeId { get; set; }
        public int ParentId { get; set; }
    }

    class UpdateOrganizationHandler : IRequestHandler<UpdateOrganizationCommand, string>
    {
        private readonly IOrganizationRepository _organizationRepo;
        private readonly ILogger<UpdateOrganizationHandler> _logger;

        public UpdateOrganizationHandler(IOrganizationRepository organizationRepository,
            ILogger<UpdateOrganizationHandler> logger)
        {
            _organizationRepo = organizationRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateOrganizationCommand request, CancellationToken cancellationToken)
        {
            var organization = await _organizationRepo.GetByIdAsync(request.Id);

            if (organization == null)
                return null;

            organization.Name = request.Name != null ? request.Name : organization.Name;
            organization.ShortName = request.ShortName != null ? request.ShortName : organization.ShortName;
            organization.Description = request.Description != null ? request.Description : organization.Description;

            if (request.OrganizationTypeId == OrganizationType.Area.Id)
            {
                organization.OrganizationTypeId = request.OrganizationTypeId;
                organization.ParentId = 1;
            }

            else if (request.OrganizationTypeId == OrganizationType.Branch.Id && request.ParentId != 0)
            {
                var org = await _organizationRepo.GetByIdAsync(request.ParentId);

                if (org == null)
                    throw new DomainException("Parent doesn't exist!");

                if (org.OrganizationTypeId != OrganizationType.Area.Id)
                    throw new DomainException("Parent Organization Type must be Area!");

                organization.OrganizationTypeId = request.OrganizationTypeId != 0 ? request.OrganizationTypeId :
                    organization.OrganizationTypeId;
                organization.ParentId = request.ParentId != 0 ? request.ParentId : organization.ParentId;
            }

            else if (request.OrganizationTypeId == OrganizationType.Sub_Branch.Id && request.ParentId != 0)
            {
                var org = await _organizationRepo.GetByIdAsync(request.ParentId);

                if (org == null)
                    throw new DomainException("Parent doesn't exist!");

                if (org.OrganizationTypeId != OrganizationType.Branch.Id)
                    throw new DomainException("Parent Organization Type must be Branch!");

                organization.OrganizationTypeId = request.OrganizationTypeId != 0 ? request.OrganizationTypeId :
                    organization.OrganizationTypeId;
                organization.ParentId = request.ParentId;
            }

            _logger.LogInformation("-----Updating Organization------");

            _organizationRepo.UpdateAsync(organization);
            await _organizationRepo.UnitOfWork.SaveChanges();

            return "Updated";
        }
    }

    public class UpdateOrganizationCommandValidator : AbstractValidator<UpdateOrganizationCommand>
    {
        public UpdateOrganizationCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} can'be null");
        }
    }
}