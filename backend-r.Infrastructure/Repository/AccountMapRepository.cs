namespace backend_r.Infrastructure.Repository
{
    public class AccountMapRepository : BaseRepository<AccountMap>, IAccountMapRepository
    {
        private readonly JoshuaContext _context;

        public AccountMapRepository(JoshuaContext joshuaContext) : base(joshuaContext)
        {
            _context = joshuaContext;
        }

        public async Task<List<AccountMap>> GetAccountMapByReferenceAndOwner(int reference, string owner)
        {
            return await _context.AccountMaps.Where(c => c.Reference == reference && c.Owner == owner)
                .Include(c => c.Account)
                .ThenInclude(c => c.AccountProductType)
                .Include(c => c.Account)
                .ThenInclude(c => c.PurchasedProducts)
                .ThenInclude(c => c.Vouchers)
                .AsSingleQuery()
                .ToListAsync();
        }

        public async Task<int> GetRefenceByAccoundIdAndOwner(int accountId, string owner)
        {
            var accountMap = await _context.AccountMaps.SingleOrDefaultAsync(c => c.AccountId == accountId
                && c.Owner == owner);
            
            if(accountMap == null)
                return -1;

            return accountMap.Reference;
        }
    }
}