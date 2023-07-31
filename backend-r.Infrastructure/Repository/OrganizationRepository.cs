namespace backend_r.Infrastructure.Repository
{
    public class OrganizationRepository : BaseRepository<Organization>, IOrganizationRepository
    {
        private readonly JoshuaContext _context;
        private readonly IUserRepository _userRepo;

        public OrganizationRepository(JoshuaContext context, IUserRepository userRepository) : base(context)
        {
            _context = context;
            _userRepo = userRepository;
        }

        public override List<string> DefaultPermission()
        {
            return new List<string>
            {
                $"AuthorizedTo.{nameof(Organization)}.Add",
                $"AuthorizedTo.{nameof(Organization)}.View",
                $"AuthorizedTo.{nameof(Organization)}.Edit",
                $"AuthorizedTo.{nameof(Organization)}.Remove",
                $"AuthorizedTo.{nameof(Organization)}.MemberGrowthsSummaryHigher",
                $"AuthorizedTo.{nameof(Organization)}.MemberCarrersSummaryHigher",
                $"AuthorizedTo.{nameof(Organization)}.Close",
                $"AuthorizedTo.{nameof(Organization)}.ShowTransaction",
                $"AuthorizedTo.{nameof(Organization)}.ClosedTransaction",
            };
        }

        public override async Task<Organization> GetByIdAsync(int id)
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if(user.Staff.StaffOrganizations.Any())
                return await _context.Organizations
                    .Include(c => c.StaffOrganizations)
                    .ThenInclude(c => c.Staff)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.IdDefinitions)
                    .Include(c => c.OrganizationType)
                    .AsSingleQuery()
                    .SingleOrDefaultAsync(c => c.Id == id && c.Id == user.Staff.StaffOrganizations[0].OrganizationId);
            else
                return await _context.Organizations
                    .Include(c => c.StaffOrganizations)
                    .ThenInclude(c => c.Staff)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.IdDefinitions)
                    .Include(c => c.OrganizationType)
                    .AsSingleQuery()
                    .SingleOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IReadOnlyList<Organization>> GetAllAsync()
        {
            var user = await _userRepo.GetAuthenticatedUser();

            if(user.Staff.StaffOrganizations.Any())
                return await _context.Organizations
                    .Include(c => c.StaffOrganizations)
                    .ThenInclude(c => c.Staff)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.IdDefinitions)
                    .Include(c => c.OrganizationType)
                    .AsSingleQuery()
                    .Where(c => c.Id == user.Staff.StaffOrganizations[0].OrganizationId)
                    .ToListAsync();
            else
                return await _context.Organizations
                    .Include(c => c.StaffOrganizations)
                    .ThenInclude(c => c.Staff)
                    .ThenInclude(c => c.Gender)
                    .Include(c => c.IdDefinitions)
                    .Include(c => c.OrganizationType)
                    .AsSingleQuery()
                    .ToListAsync();
        }

        public async Task<List<Organization>> ListofMembers()
        {
            return await _context.Organizations
                .Include(c => c.MemberOrganizations)
                .ThenInclude(c => c.Member)
                .ToListAsync();
        }

        public async Task<List<Organization>> ListofMembersOccupations()
        {
            return await _context.Organizations
                .Include(c => c.MemberOrganizations)
                .ThenInclude(c => c.Member)
                .ThenInclude(c => c.Occupations)
                .ToListAsync();
        }
    }
}