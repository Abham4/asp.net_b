namespace backend_r.Application.Common.Commands.Staff.Identity
{
    public class CreateStaffIdentity : IRequest<string>
    {
        public int Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
        public int StaffId { get; set; }
    }

    public class CreateStaffIdentityHandler : IRequestHandler<CreateStaffIdentity, string>
    {
        private readonly IStaffRepository _staffRepo;
        private readonly IIdentityRepository _identityRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateStaffIdentityHandler> _logger;

        public CreateStaffIdentityHandler(IStaffRepository staffRepository, IIdentityRepository identityRepository,
            ILogger<CreateStaffIdentityHandler> logger, IUserRepository userRepository)
        {
            _staffRepo = staffRepository;
            _identityRepo = identityRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateStaffIdentity request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var staff = await _staffRepo.CheckExistence(request.StaffId);

            if (!staff)
                throw new DomainException("Related staff doesn't exist!");

            var type = IdentityType.List().SingleOrDefault(c => c.Id == request.Type).Name;

            var identity = new Domain.Entities.Identity("Staff", type, request.Description, request.Number, request.StaffId,
                user.Email);

            _logger.LogInformation("----------Creating staff {0} identity----------", request.StaffId);

            await _identityRepo.AddAsync(identity);
            await _identityRepo.UnitOfWork.SaveChanges();

            return "Created.";
        }
    }

    public class CreateStaffIdentityValidator : AbstractValidator<CreateStaffIdentity>
    {
        public CreateStaffIdentityValidator()
        {
            RuleFor(c => c.StaffId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Type)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3).WithMessage("{PropertyName} is null reference!");

            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.Number)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}