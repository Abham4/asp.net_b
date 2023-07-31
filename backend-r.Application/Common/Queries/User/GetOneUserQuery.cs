namespace backend_r.Application.Common.Queries.User
{
    public class GetOneUserQuery : IRequest<OneUserVm>
    {
        public int Id { get; set; }
    }

    public class GetOneUserHandler : IRequestHandler<GetOneUserQuery, OneUserVm>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<GetOneUserHandler> _logger;

        public GetOneUserHandler(IUserRepository userRepository, ILogger<GetOneUserHandler> logger)
        {
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<OneUserVm> Handle(GetOneUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetUser(request.Id);

            if (user == null)
                return null;

            var userRoles = new List<Vms.UserRole>();
            var roleClaims = new List<RoleClaimsVm>();

            for (int i = 0; i < user.UserRoles.Count(); i++)
            {
                foreach (var roleClaim in user.UserRoles[i].Role.RoleClaims)
                {
                    roleClaims.Add(new RoleClaimsVm{
                        Value = roleClaim.ClaimValue
                    });
                }

                userRoles.Add(new Vms.UserRole
                {
                    Id = user.UserRoles[i].Role.Id,
                    Name = user.UserRoles[i].Role.Name,
                    RoleClaims = roleClaims
                });

                roleClaims.Clear();
            }

            var organizaion = user.Staff.StaffOrganizations.Count() > 0 ? new OneOrganizationVm{
                Id = user.Staff.StaffOrganizations[0].Organization.Id,
                Name = user.Staff.StaffOrganizations[0].Organization.Name
            } : null;

            var oneUser = new OneUserVm
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles,
                CreatedDate = user.CreatedDate,
                LastModifiedDate = user.LastModifiedDate,
                Staff = new OneStaffVm
                {
                    Id = user.Staff.Id,
                    Title = user.Staff.Title,
                    FirstName = user.Staff.FirstName,
                    MiddleName = user.Staff.MiddleName,
                    LastName = user.Staff.LastName,
                    Organization = organizaion,
                    ProfileImg = user.Staff.ProfileImg,
                    Gender = new Staff.Vms.Gender
                    {
                        Id = user.Staff.Gender.Id,
                        Name = user.Staff.Gender.Name,
                    }
                }
            };

            _logger.LogInformation("---------Getting User By Id--------{Email}-------", user.Email);

            return oneUser;
        }
    }
}