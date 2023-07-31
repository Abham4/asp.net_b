namespace backend_r.Infrastructure.Repository
{
    public class PassBookRepository : BaseRepository<PassBook>, IPassBookRepository 
    {
        private readonly JoshuaContext _context;
        public PassBookRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public bool PassBookExist(string passBook)
        {
            return _context.PassBooks.Any(c => c.Code == passBook);
        }
    }
}