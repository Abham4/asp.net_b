namespace backend_r.Infrastructure.Repository
{
    public class StaffRepository : BaseRepository<Staff>, IStaffRepository
    {
        private readonly JoshuaContext _context;
        public StaffRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public override List<string> DefaultPermission()
        {
            return new List<string>
            {
                $"AuthorizedTo.{nameof(Staff)}.Add",
                $"AuthorizedTo.{nameof(Staff)}.View",
                $"AuthorizedTo.{nameof(Staff)}.Edit",
                $"AuthorizedTo.{nameof(Staff)}.Remove",
                $"AuthorizedTo.{nameof(Staff)}.Unregistered"
            };
        }

        public bool DoesStaffConnectedToOrganization(int id)
        {
            return _context.StaffOrganizations.Any(c => c.StaffId == id);
        }

        public override async Task<IReadOnlyList<Staff>> GetAllAsync()
        {
            return await _context.Staffs
                .Include(c => c.StaffOrganizations)
                .ThenInclude(c => c.Organization)
                .Include(c => c.Gender)
                .AsSingleQuery()
                .ToListAsync();
        }

        public override async Task<Staff> GetByIdAsync(int id)
        {
            return await _context.Staffs
                .Include(c => c.StaffOrganizations)
                .ThenInclude(c => c.Organization)
                .Include(c => c.Gender)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Staff>> UnRegisteredStaffs()
        {
            return await _context.Staffs.Include(c => c.Gender).Where(c => c.User == null).ToListAsync();
        }
    }
}