namespace backend_r.Application.Common.Commands.User
{
    public class UpdateCommand : IRequest<string>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<UserRole> UserRoles { get; set; }
    }

    public class UpdateCommandHandler : IRequestHandler<UpdateCommand, string>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UpdateCommandHandler> _logger;

        public UpdateCommandHandler(IUserRepository userRepository, ILogger<UpdateCommandHandler> logger)
        {
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(UpdateCommand request, CancellationToken cancellationToken)
        {
            var authenticatedUser = await _userRepo.GetAuthenticatedUser();
            var userRoles = new List<Domain.Entities.UserRole>{
                new Domain.Entities.UserRole{
                    RoleId = request.UserRoles[0].RoleId
                }
            };
            var user = new Domain.Entities.User(request.Id, request.Email, request.PhoneNumber, userRoles,
                request.ConfirmPassword, authenticatedUser.Email);
            var result = await _userRepo.Modify(user);

            _logger.LogInformation("--------Updating User---------{Email}-------", user.Email);

            return result;
        }
    }

    public class UpdateCommandValidator : AbstractValidator<UpdateCommand>
    {
        public UpdateCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.Email)
                .EmailAddress();

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .Length(8, 20)
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ConfirmPassword)
                .Equal(c => c.Password).WithMessage("{PropertyName} doesn't match with Password");
        }
    }
}