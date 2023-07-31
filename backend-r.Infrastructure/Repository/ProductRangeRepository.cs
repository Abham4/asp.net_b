namespace backend_r.Infrastructure.Repository
{
    public class ProductRangeRepository : BaseRepository<ProductRange>, IProductRangeRepository
    {
        public ProductRangeRepository(JoshuaContext joshuaContext) : base(joshuaContext) {}
    }
}