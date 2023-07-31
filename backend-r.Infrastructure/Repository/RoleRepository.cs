namespace backend_r.Infrastructure.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private IUnitOfWork unitOfWork;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (unitOfWork == null)
                {
                    unitOfWork = new UnitOfWork(this._context);
                }
                return unitOfWork;
            }
            set
            {
                unitOfWork = new UnitOfWork(this._context);
            }
        }

        private readonly JoshuaContext _context;
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(JoshuaContext context, RoleManager<Role> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task AddAsync(Role model)
        {
            await _context.Roles.AddAsync(model);
        }

        public async Task<IReadOnlyList<Role>> GetAllAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            return await _context.Roles
                .Include(c => c.UserRoles)
                .ThenInclude(c => c.User)
                .ThenInclude(c => c.Member)
                .Include(c => c.UserRoles)
                .ThenInclude(c => c.User)
                .ThenInclude(c => c.Staff)
                .Include(c => c.RoleClaims)
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Role> GetByName(string roleName)
        {
            return await _context.Roles.SingleOrDefaultAsync(c => c.Name == roleName);
        }

        public List<string> DefaultPermission()
        {
            return new List<string>
            {
                $"AuthorizedTo.{nameof(Role)}.Add",
                $"AuthorizedTo.{nameof(Role)}.View",
                $"AuthorizedTo.{nameof(Role)}.Edit",
                $"AuthorizedTo.{nameof(Role)}.Remove",
            };
        }

        public void Modify(Role model)
        {
            if (model == null)
                throw new ArgumentNullException("entity");

            _context.Entry(model).State = EntityState.Modified;
            _context.Set<Role>().Update(model);
        }

        public async Task<bool> CheckExistence(int id)
        {
            return await _context.Roles.AnyAsync(c => c.Id == id);
        }
    }
}