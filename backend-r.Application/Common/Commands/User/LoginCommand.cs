namespace backend_r.Application.Common.Commands.User
{
    public class LoginCommand : IRequest<string>
    {
        public string EmailorPhone { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IUserRepository userRepository, ILogger<LoginCommandHandler> logger)
        {
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {   
            var response = await _userRepo.Login(request.EmailorPhone, request.Password);

            _logger.LogInformation("------User Logging In---{1}---{2}", request.EmailorPhone, response);
            
            return response;
        }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(c => c.EmailorPhone)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.Password)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }
    }
}