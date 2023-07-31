namespace backend_r.Application.Common.Queries.PurchasedProduct
{
    public class GetOnePurchasedProductQuery : IRequest<OnePurchasedProductVm>
    {
        public int Id { get; set; }
    }

    public class GetOnePurchasedProductQueryHandler : IRequestHandler<GetOnePurchasedProductQuery, 
        OnePurchasedProductVm>
    {
        private readonly IPurchasedProductRepository _purchasedProductRepo;
        private readonly ILogger<GetOnePurchasedProductQueryHandler> _logger;

        public GetOnePurchasedProductQueryHandler(IPurchasedProductRepository purchasedProductRepository, 
            ILogger<GetOnePurchasedProductQueryHandler> logger)
        {
            _logger = logger;
            _purchasedProductRepo = purchasedProductRepository;
        }

        public async Task<OnePurchasedProductVm> Handle(GetOnePurchasedProductQuery request, 
            CancellationToken cancellationToken)
        {
            var purchasedProduct = await _purchasedProductRepo.GetByIdAsync(request.Id);
            
            _logger.LogInformation("--------Getting purchased product---------");
            
            if(purchasedProduct == null)
                return null;

            var onePurchasedProduct = new Vms.OnePurchasedProductVm
            {
                Id = purchasedProduct.Id,
                PurchasedDate = purchasedProduct.PurchasedDate,
                MaturityDate = purchasedProduct.MaturityDate,
                EndDate = purchasedProduct.EndDate,
                Product = new Vms.Product
                    {
                        Description = purchasedProduct.Product.Description
                    },
                ProductSetups = purchasedProduct.ProductSetups.Select(d => new Vms.ProductSetup{
                    Amount = d.Amount,
                    PaymentCount = d.PaymentCount,
                    PayCycle = d.PayCycle,
                    InterestRate = d.InterestRate,
                    PreDepositAmount = d.PreDepositAmount,
                    PaymentStartDate = d.PaymentStartDate,
                    LastObjectState = d.LastObjectState
                }).ToList(),
                ScheduleHeaders = purchasedProduct.ScheduleHeaders.Select(e => new Vms.ScheduleHeader
                    {
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        ScheduleDate = e.ScheduleDate,
                        LastObjectState = e.LastObjectState,
                        ProductSchedules = e.ProductSchedules.Select(f => new Vms.ProductSchedule{
                            DateExpected = f.DateExpected,
                            PricipalDue = f.PricipalDue,
                            InterestDue = f.InterestDue,
                            Status = f.Status,
                        }).ToList()
                    }).ToList(),
                Vouchers = purchasedProduct.Vouchers.Select(c => new Vms.Voucher{
                        Code = c.Code,
                        VoucherTypeName = c.VoucherType.Name,
                        TimeStamp = c.TimeStamp,
                        AmountTransacted = c.AmountTransacted,
                        MemberCode = c.Member,
                        Remark = c.Remark
                    }).ToList()
            };

            return onePurchasedProduct;
        }
    }

    public class GetOnePurchasedProductQueryValidator : AbstractValidator<GetOnePurchasedProductQuery>
    {
        public GetOnePurchasedProductQueryValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}