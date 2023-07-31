namespace backend_r.Infrastructure.Repository
{
    public class IdDefinitionRepository : BaseRepository<IdDefinition> , IIdDefinitionRepository
    {
        private readonly JoshuaContext _context;
        public IdDefinitionRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<IdDefinition>> GetByOrganizationId(int id)
        {
            return await _context.IdDefinitions.Where(c => c.OrganizationId == id).ToListAsync();
        }

        public async Task<IdDefinition> GetByResourseTypeAndOrganizationId(int id, string resourceType)
        {
            return await _context.IdDefinitions.SingleOrDefaultAsync(c => c.OrganizationId == id && c.Resource.Type == resourceType);
        }
    }
}
