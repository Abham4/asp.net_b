namespace backend_r.Infrastructure.Repository
{
    public class SpouseRepository : BaseRepository<Spouse> , ISpouseRepository
    {        
        public SpouseRepository(JoshuaContext context) : base(context) {}
    }
}
