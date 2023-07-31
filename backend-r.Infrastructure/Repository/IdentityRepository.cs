namespace backend_r.Infrastructure.Repository
{
    public class IdentityRepository : BaseRepository<Identity> , IIdentityRepository
    {
        private readonly JoshuaContext _context;
        public IdentityRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Identity>> GetByReferenceAndOwnerType(int reference, string owner)
        {
            return await _context.Identities.Where(c => c.Reference == reference && c.Owner == owner).ToListAsync();
        }
    }
}
