namespace backend_r.Application.Common.Commands.Staff.Identity
{
    public class UpdateStaffIdentity : IRequest<string>
    {
        public int Id { get; set; }
        public int Reference { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }

    public class UpdateStaffIdentityHandler : IRequestHandler<UpdateStaffIdentity, string>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IIdentityRepository _identityRepo;
        private readonly ILogger<UpdateStaffIdentityHandler> _logger;

        public UpdateStaffIdentityHandler(IStaffRepository staffRepository, IIdentityRepository identityRepository,
            ILogger<UpdateStaffIdentityHandler> logger)
        {
            _staffRepo = staffRepository;
            _identityRepo = identityRepository;
            _logger = logger;
        }

        public async Task<string> Handle(UpdateStaffIdentity request, CancellationToken cancellationToken)
        {
            var identity = await _identityRepo.GetByIdAsync(request.Id);
            var staff = await _staffRepo.CheckExistence(request.Reference);

            if (identity == null)
                return null;

            if (!staff)
                throw new DomainException("Related Staff doesn't exist!");

            identity.Type = request.Type != null ? request.Type : identity.Type;
            identity.Description = request.Description != null ? request.Description : identity.Description;
            identity.Number = request.Number != null ? request.Number : identity.Number;

            _logger.LogInformation("--------Updating staff {0} identity---------", request.Reference);

            _identityRepo.UpdateAsync(identity);
            await _identityRepo.UnitOfWork.SaveChanges();

            return "Updated.";
        }
    }

    public class UpdateStaffIdentityValidator : AbstractValidator<UpdateStaffIdentity>
    {
        public UpdateStaffIdentityValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Reference)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}