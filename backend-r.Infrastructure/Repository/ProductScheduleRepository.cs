namespace backend_r.Infrastructure.Repository
{
    public class ProductScheduleRepository : BaseRepository<ProductSchedule>, IProductScheduleRepository
    {
        public ProductScheduleRepository(JoshuaContext joshuaContext) : base(joshuaContext) {}
    }
}