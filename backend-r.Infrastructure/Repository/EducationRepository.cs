namespace backend_r.Infrastructure.Repository
{
    public class EducationRepository : BaseRepository<Education>, IEducationRepository
    {
        private JoshuaContext _context;
        public EducationRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }
    }
}
