namespace backend_r.Infrastructure.Repository
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly JoshuaContext _context;
        public ProductRepository(JoshuaContext joshuaContext) : base(joshuaContext)
        {
            _context = joshuaContext;
        }

        public async override Task<IReadOnlyList<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(c => c.AccountProductType)
                .Include(c => c.LoanExtension)
                .Include(c => c.SaveExtention)
                .Include(c => c.ShareExtension)
                .AsSingleQuery()
                .ToListAsync();
        }

        public async override Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(c => c.AccountProductType)
                .Include(c => c.LoanExtension)
                .Include(c => c.SaveExtention)
                .Include(c => c.ShareExtension)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Id == id);
        }
    }
}