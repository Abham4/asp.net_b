namespace backend_r.Application.Common.Queries.Role
{
    public class GetOneRoleQuery : IRequest<OneRoleVm>
    {
        public int Id { get; set; }
    }

    public class GetOneRoleHandler : IRequestHandler<GetOneRoleQuery, OneRoleVm>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly ILogger<GetOneRoleHandler> _logger;

        public GetOneRoleHandler(IRoleRepository roleRepository, ILogger<GetOneRoleHandler> logger)
        {
            _logger = logger;
            _roleRepo = roleRepository;
        }

        public async Task<OneRoleVm> Handle(GetOneRoleQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleRepo.GetByIdAsync(request.Id);
            var names = "";
            var roleClaims = new List<Vms.RoleClaimsVm>();

            if (role == null)
                return null;
            
            for (int i = 0; i < role.UserRoles.Count(); i++)
            {
                names += role.UserRoles[i].User.Member != null ? role.UserRoles[i].User.Member.FirstName
                    + " " + role.UserRoles[i].User.Member.LastName + "," : role.UserRoles[i].User.Staff.FirstName
                    + " " + role.UserRoles[i].User.Staff.LastName + ",";
            }

            foreach (var roleClaim in role.RoleClaims)
            {
                roleClaims.Add(new RoleClaimsVm{
                    Value = roleClaim.ClaimValue
                });
            }

            var oneRole = new OneRoleVm
            {
                Name = role.Name,
                CreatedDate = role.CreatedDate,
                Users = role.UserRoles.Count() > 0 ? names : null,
                RoleClaims = roleClaims
            };

            _logger.LogInformation("-------Getting Role---------");

            return oneRole;
        }
    }
}