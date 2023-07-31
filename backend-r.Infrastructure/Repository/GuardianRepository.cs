namespace backend_r.Infrastructure.Repository
{
    public class GuardianRepository : BaseRepository<Guardian>, IGuardianRepository
    {
        public GuardianRepository(JoshuaContext context) : base(context) {}
    }
}