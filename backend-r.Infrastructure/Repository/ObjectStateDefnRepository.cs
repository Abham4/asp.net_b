namespace backend_r.Infrastructure.Repository
{
    public class ObjectStateDefnRepository : BaseRepository<ObjectStateDefn>, IObjectStateDefnRepository
    {
        private readonly JoshuaContext _context;
        public ObjectStateDefnRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ObjectStateDefn> GetObjectStateDefnByResourseTypeAndName(string resourceType, string name)
        {
            return await _context.ObjectStateDefns
                .Include(c => c.Resource)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Resource.Type == resourceType && c.Name == name);
        }
    }
}