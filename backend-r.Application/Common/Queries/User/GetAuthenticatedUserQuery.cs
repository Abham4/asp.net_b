namespace backend_r.Application.Common.Queries.User
{
    public class GetAuthenticatedUserQuery : IRequest<OneUserVm> {}

    public class GetAuthenticatedUserHandler : IRequestHandler<GetAuthenticatedUserQuery, OneUserVm>
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<GetOneUserHandler> _logger;

        public GetAuthenticatedUserHandler(IUserRepository userRepository, ILogger<GetOneUserHandler> logger)
        {
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<OneUserVm> Handle(GetAuthenticatedUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            _logger.LogInformation("---------Getting Authenticated User--------{Email}-------", user.Email);

            var userRoles = new List<Vms.UserRole>();
            var roleClaims = new List<RoleClaimsVm>();

            for (int i = 0; i < user.UserRoles.Count(); i++)
            {
                foreach (var roleClaim in user.UserRoles[i].Role.RoleClaims)
                {
                    roleClaims.Add(new RoleClaimsVm
                    {
                        Value = roleClaim.ClaimValue
                    });
                }

                userRoles.Add(new Vms.UserRole
                {
                    Id = user.UserRoles[i].Role.Id,
                    Name = user.UserRoles[i].Role.Name,
                    RoleClaims = roleClaims
                });
            }

            var organizaion = user.Staff.StaffOrganizations.Count() > 0 ? new OneOrganizationVm{
                Id = user.Staff.StaffOrganizations[0].Organization.Id,
                Name = user.Staff.StaffOrganizations[0].Organization.Name
            } : null;

            var oneUser = new Vms.OneUserVm
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Roles = userRoles,
                CreatedDate = user.CreatedDate,
                LastModifiedDate = user.LastModifiedDate,
                Staff = user.Staff != null ? new OneStaffVm{
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
                } : null
            };

            return oneUser;
        }
    }
}