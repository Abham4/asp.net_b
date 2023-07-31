namespace backend_r.Application.Common.Queries.User
{
    public class GetAllUserQuery : IRequest<IEnumerable<AllUserVm>> {}

    public class GetAllUserHandler : IRequestHandler<GetAllUserQuery, IEnumerable<AllUserVm>>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<GetOneUserHandler> _logger;

        public GetAllUserHandler(IUserRepository userRepository, ILogger<GetOneUserHandler> logger)
        {
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<IEnumerable<AllUserVm>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
        {
            var users = await _userRepo.GetUsers();

            _logger.LogInformation("---------Getting All User--------");

            return users.Select(c => new AllUserVm
            {
                Id = c.Id,
                FirstName = c.Member != null ? c.Member.FirstName : c.Staff.FirstName,
                LastName = c.Member != null ? c.Member.LastName : c.Staff.LastName,
                Email = c.Email,
                PhoneNumber = c.PhoneNumber,
                CreatedDate = c.CreatedDate,
                LastModifiedDate = c.LastModifiedDate
            });
        }
    }
}