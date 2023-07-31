namespace backend_r.Application.Common.Queries.Role
{
    public class GetAllRoleQuery : IRequest<IEnumerable<AllRoleVm>> {}

    public class GetAllRoleHandler : IRequestHandler<GetAllRoleQuery, IEnumerable<AllRoleVm>>
    {
        private readonly IRoleRepository _roleRepo;
        private readonly ILogger<GetAllRoleHandler> _logger;

        public GetAllRoleHandler(IRoleRepository roleRepository, ILogger<GetAllRoleHandler> logger)
        {
            _roleRepo = roleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllRoleVm>> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepo.GetAllAsync();

            _logger.LogInformation("-------Getting Roles---------");

            return roles.Select(c => new AllRoleVm
            {
                Id = c.Id,
                Name = c.Name,
                CreatedDate = c.CreatedDate
            });
        }
    }
}