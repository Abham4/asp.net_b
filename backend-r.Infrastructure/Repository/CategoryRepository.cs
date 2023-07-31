namespace backend_r.Infrastructure.Repository
{
    public class CategoryRepository : BaseRepository<Category> , ICategoryRepository
    {
        private readonly JoshuaContext _context;
        public CategoryRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }
    }
}
