namespace backend_r.Application.Common.Commands.User
{
    public class LogoutCommand : IRequest<string> {}

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, string>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(IUserRepository userRepository, ILogger<LogoutCommandHandler> logger)
        {
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var authenticatedUser = await _userRepo.GetAuthenticatedUser();
            var email = authenticatedUser.Email;

            var response = await _userRepo.Logout(authenticatedUser);

            _logger.LogInformation("-------User Logging out------{@email}-------", email);

            return response;
        }
    }
}