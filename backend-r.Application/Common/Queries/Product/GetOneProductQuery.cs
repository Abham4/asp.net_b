namespace backend_r.Application.Common.Queries.Product
{
    public class GetOneProductQuery : IRequest<OneProductVm>
    {
        public int Id { get; set; }
    }

    public class GetOneProductQueryHandler : IRequestHandler<GetOneProductQuery, OneProductVm>
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductRangeRepository _productRangeRepo;
        private readonly ILogger<GetOneProductQueryHandler> _logger;

        public GetOneProductQueryHandler(IProductRepository productRepository,
            IProductRangeRepository productRangeRepository, ILogger<GetOneProductQueryHandler> logger)
        {
            _productRepo = productRepository;
            _productRangeRepo = productRangeRepository;
            _logger = logger;
        }

        public async Task<OneProductVm> Handle(GetOneProductQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.Id);

            if (product == null)
                return null;

            var pricipalRange = new Domain.Entities.ProductRange();
            var repaymentRange = new Domain.Entities.ProductRange();
            var interestRateRange = new Domain.Entities.ProductRange();
            var repaymentCycleRange = new Domain.Entities.ProductRange();
            var sharesPerMemberRange = new Domain.Entities.ProductRange();

            if (product.LoanExtension != null)
            {
                pricipalRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.PricipalRange);
                repaymentRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.RepaymentRange);
                interestRateRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.InterestRateRange);
                repaymentCycleRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.RepayCycleRange);
            }

            if (product.ShareExtension != null)
                sharesPerMemberRange = await _productRangeRepo.GetByIdAsync(product.ShareExtension.SharesPerMemberRange);

            _logger.LogInformation("-------Getting Product--------");

            var oneProduct = new OneProductVm
            {
                Id = product.Id,
                ProductTypeName = product.AccountProductType.Name,
                Description = product.Description,
                StartDate = product.StartDate,
                ClosedDate = product.ClosedDate,
                LoanExtension = product.LoanExtension != null ? new Vms.LoanExtension
                {
                    NoOfRepayment = product.LoanExtension.NoOfRepayment,
                    MemebershipMonth = product.LoanExtension.MembershipMonth,
                    SaveSharePercentage = product.LoanExtension.SaveSharePercentage,
                    PricipalMin = pricipalRange.Min,
                    PricipalDefault = pricipalRange.DefaultValue,
                    PricipalMax = pricipalRange.Max,
                    RepaymentMin = repaymentRange.Min,
                    RepaymentDefault = repaymentRange.DefaultValue,
                    RepaymentMax = repaymentRange.Max,
                    InterestRateMin = interestRateRange.Min,
                    InterestRateDefault = interestRateRange.DefaultValue,
                    InterestRateMax = interestRateRange.Max,
                    RepayCycleMin = repaymentCycleRange.Min,
                    RepayCycleDefault = repaymentCycleRange.DefaultValue,
                    RepayCycleMax = repaymentCycleRange.Max
                } : null,
                SaveExtension = product.SaveExtention != null ? new Vms.SaveExtension
                {
                    MinOpeningBalance = product.SaveExtention.MinOpeningBalance,
                    MinRequiredBalance = product.SaveExtention.MinRequiredBalance,
                    MinCompulsoryAmount = product.SaveExtention.MinCompulsoryAmount,
                    PayCycle = product.SaveExtention.PayCycle,
                    // PenalityRate = product.SaveExtention.PenalityRate,
                    // PenalityAmount = product.SaveExtention.PenalityAmount,
                    InterestRate = product.SaveExtention.InterestRate,
                    InterestTaxRate = product.SaveExtention.InterestTaxRate
                } : null,
                ShareExtension = product.ShareExtension != null ? new Vms.ShareExtension
                {
                    TotalShareCount = product.ShareExtension.TotalShareCount,
                    NominalPrice = product.ShareExtension.NominalPrice,
                    SharesToBeIssued = product.ShareExtension.SharesToBeIssued,
                    CapitalValue = product.ShareExtension.CapitalValue,
                    SharesPerMemberMin = sharesPerMemberRange.Min,
                    SharesPerMemberDefault = sharesPerMemberRange.DefaultValue,
                    SharesPerMemberMax = sharesPerMemberRange.Max
                } : null
            };

            return oneProduct;
        }
    }
}