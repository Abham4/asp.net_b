namespace backend_r.Application.Common.Queries.PurchasedProduct
{
    public class GetAllPurchasedProductQuery : IRequest<IEnumerable<AllPurchasedProductVm>> {}

    public class GetAllPurchasedProductQueryHandler : IRequestHandler<GetAllPurchasedProductQuery, IEnumerable<AllPurchasedProductVm>>
    {
        private readonly IPurchasedProductRepository _purchasedProductRepo;
        private readonly ILogger<GetAllPurchasedProductQueryHandler> _logger;

        public GetAllPurchasedProductQueryHandler(IPurchasedProductRepository purchasedProductRepository, 
            ILogger<GetAllPurchasedProductQueryHandler> logger)
        {
            _logger = logger;
            _purchasedProductRepo = purchasedProductRepository;
        }

        public async Task<IEnumerable<AllPurchasedProductVm>> Handle(GetAllPurchasedProductQuery request,
            CancellationToken cancellationToken)
        {
            var purchasedProducts = await _purchasedProductRepo.GetAllAsync();

            _logger.LogInformation("-------Getting all purchased product---------");

            return purchasedProducts.Select(c => new AllPurchasedProductVm{
                Id = c.Id,
                PurchasedDate = c.PurchasedDate,
                MaturityDate = c.MaturityDate,
                EndDate = c.EndDate,
                ProductSetups = c.ProductSetups.Select(e => new Vms.ProductSetup{
                    Amount = e.Amount,
                    PaymentCount = e.PaymentCount,
                    PayCycle = e.PayCycle,
                    InterestRate = e.InterestRate,
                    PreDepositAmount = e.PreDepositAmount,
                    PaymentStartDate = e.PaymentStartDate,
                    LastObjectState = e.LastObjectState
                }).ToList()
            });
        }
    }
}