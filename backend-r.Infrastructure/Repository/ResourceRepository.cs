namespace backend_r.Infrastructure.Repository
{
    public class ResourceRepository : BaseRepository<Resource>, IResourceRepository
    {
        private readonly JoshuaContext _context;
        public ResourceRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Resource> GetResourceByType(string type)
        {
            return await _context.Resources.SingleOrDefaultAsync(c => c.Type == type);
        }
    }
}
