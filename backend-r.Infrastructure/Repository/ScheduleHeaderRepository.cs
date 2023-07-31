namespace backend_r.Infrastructure.Repository
{
    public class ScheduleHeaderRepository : BaseRepository<ScheduleHeader>, IScheduleHeaderRepository
    {
        public ScheduleHeaderRepository(JoshuaContext joshuaContext) : base(joshuaContext) {}
    }
}