namespace backend_r.Infrastructure.Repository
{
    public class PurchasedProductRepository : BaseRepository<PurchasedProduct>, IPurchasedProductRepository
    {
        private readonly JoshuaContext _context;
        private readonly IUserRepository _userRepo;
        public PurchasedProductRepository(JoshuaContext joshuaContext, IUserRepository userRepository) : base(joshuaContext)
        {
            _context = joshuaContext;
            _userRepo = userRepository;
        }

        public async override Task<IReadOnlyList<PurchasedProduct>> GetAllAsync()
        {
            return await _context.PurchasedProducts
                .Include(c => c.ProductSetups)
                .AsSingleQuery()
                .ToListAsync();
        }

        public async override Task<PurchasedProduct> GetByIdAsync(int id)
        {
            return await _context.PurchasedProducts
                .Include(c => c.Account)
                .ThenInclude(c => c.AccountProductType)
                .Include(c => c.ProductSetups)
                .Include(c => c.Product)
                .Include(c => c.Vouchers)
                .ThenInclude(e => e.VoucherType)
                .Include(c => c.ScheduleHeaders)
                .ThenInclude(c => c.ProductSchedules)
                .AsSingleQuery()
                .SingleOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<PurchasedProduct>> GetPurchasedProductsByAccountId(int id)
        {
            return await _context.PurchasedProducts
                .Include(c => c.Account)
                .ThenInclude(c => c.AccountProductType)
                .Include(c => c.ProductSetups)
                .Include(c => c.Product)
                .Include(c => c.Vouchers)
                .ThenInclude(e => e.VoucherType)
                .Include(c => c.ScheduleHeaders)
                .ThenInclude(c => c.ProductSchedules)
                .AsSingleQuery()
                .Where(c => c.AccountId == id).ToListAsync();
        }

        public int NoofPrchasedShare(int accountId, int productId)
        {
            return _context.PurchasedProducts.Where(c => c.AccountId == accountId && c.ProductId == productId).Count();
        }

        public async Task Scheduler(AccountProductType type, int paymentCount, int scheduleHeaderId, DateTime startingDate,
            double principalDue, double interest, double remain, float payCycle, double due, double rate)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            if (type.Id == AccountProductType.Loan.Id)
            {
                for (int i = 0; i < paymentCount - 1; i++)
                {
                    var schedule = new Domain.Entities.ProductSchedule(scheduleHeaderId, startingDate,
                        principalDue, interest, false, user.Email);

                    await _context.AddAsync(schedule);
                    await UnitOfWork.SaveChanges();

                    interest = Math.Round(remain * rate, 2);
                    principalDue = Math.Round(due - interest, 2);
                    remain = Math.Round(remain - principalDue, 2);
                    startingDate = startingDate.AddDays(payCycle);
                }

                principalDue = Math.Round(principalDue + remain, 2);

                var lastSchedule = new Domain.Entities.ProductSchedule(scheduleHeaderId, startingDate,
                    principalDue, interest, false, user.Email);

                await _context.AddAsync(lastSchedule);
                await UnitOfWork.SaveChanges();
            }

            else if (type.Id == AccountProductType.Sharing.Id)
            {
                for (int i = 0; i < paymentCount - 1; i++)
                {
                    var schedule = new Domain.Entities.ProductSchedule(scheduleHeaderId, startingDate,
                        principalDue, false, user.Email);

                    await _context.AddAsync(schedule);
                    await UnitOfWork.SaveChanges();

                    startingDate = startingDate.AddDays(payCycle);
                }

                principalDue = Math.Round(principalDue + remain, 2);

                var lastSchedule = new Domain.Entities.ProductSchedule(scheduleHeaderId, startingDate,
                    principalDue, false, user.Email);

                await _context.AddAsync(lastSchedule);
                await UnitOfWork.SaveChanges();
            }

            else if (type.Id == AccountProductType.Saving.Id)
            {
                if (payCycle > 365.2425)
                    throw new DomainException("Must be lessthan 1 year!");

                var iterator = 365.2425 / payCycle;

                for (int i = 0; i < iterator; i++)
                {
                    var schedule = new Domain.Entities.ProductSchedule(scheduleHeaderId, startingDate,
                        principalDue, false, user.Email);

                    await _context.AddAsync(schedule);
                    await UnitOfWork.SaveChanges();

                    startingDate = startingDate.AddDays(payCycle);
                }
            }
        }

        public async Task<double> TotalPurchasedAmount(int accountId)
        {
            var purchasedProduct = await _context.PurchasedProducts.Where(c => c.AccountId == accountId)
                .Include(c => c.ProductSetups)
                .ToListAsync();
            var totalCount = 0.0;

            foreach (var item in purchasedProduct)
            {
                totalCount += item.ProductSetups.Where(c => c.LastObjectState == ObjectStateEnumeration.Active.Name)
                    .Sum(e => e.Amount);
            }

            return totalCount;
        }
    }
}