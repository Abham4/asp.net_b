namespace backend_r.Infrastructure.Repository
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        private readonly JoshuaContext _context;
        public AccountRepository(JoshuaContext context) : base(context)
        {
            _context = context;
        }

        public async override Task<Account> GetByIdAsync(int id)
        {
            return await _context.Accounts
                .Include(c => c.AccountProductType)
                .Include(c => c.PurchasedProducts)
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async override Task<IReadOnlyList<Account>> GetAllAsync()
        {
            return await _context.Accounts
                .Include(c => c.AccountProductType)
                .Include(c => c.PurchasedProducts)
                .ToListAsync();
        }
    }
}
