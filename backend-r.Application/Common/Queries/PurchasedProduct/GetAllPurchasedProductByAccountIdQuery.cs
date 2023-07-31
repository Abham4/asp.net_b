namespace backend_r.Application.Common.Queries.PurchasedProduct
{
    public class GetAllPurchasedProductByAccountIdQuery : IRequest<IEnumerable<OnePurchasedProductVm>>
    {
        public int AccountId { get; set; }
    }

    public class GetAllPurchasedProductByAccountIdQueryHandler : IRequestHandler<GetAllPurchasedProductByAccountIdQuery,
        IEnumerable<OnePurchasedProductVm>>
    {
        private readonly IPurchasedProductRepository _purchasedProductRepo;
        private readonly ILogger<GetAllPurchasedProductByAccountIdQueryHandler> _logger;

        public GetAllPurchasedProductByAccountIdQueryHandler(IPurchasedProductRepository purchasedProductRepository,
            ILogger<GetAllPurchasedProductByAccountIdQueryHandler> logger)
        {
            _logger = logger;
            _purchasedProductRepo = purchasedProductRepository;
        }

        public async Task<IEnumerable<OnePurchasedProductVm>> Handle(GetAllPurchasedProductByAccountIdQuery request,
            CancellationToken cancellationToken)
        {
            var purchasedProducts = await _purchasedProductRepo.GetPurchasedProductsByAccountId(request.AccountId);
            
            if(purchasedProducts.Count() == 0)
                return null;

            _logger.LogInformation("--------Getting purchased products by account id---------");

            return purchasedProducts.Select(c => new OnePurchasedProductVm
            {
                Id = c.Id,
                PurchasedDate = c.PurchasedDate,
                MaturityDate = c.MaturityDate,
                EndDate = c.EndDate,
                Product = new Vms.Product
                    {
                        Description = c.Product.Description
                    },
                ProductSetups = c.ProductSetups.Select(d => new Vms.ProductSetup{
                    Amount = d.Amount,
                    PaymentCount = d.PaymentCount,
                    PayCycle = d.PayCycle,
                    InterestRate = d.InterestRate,
                    PreDepositAmount = d.PreDepositAmount,
                    PaymentStartDate = d.PaymentStartDate,
                    LastObjectState = d.LastObjectState
                }).ToList(),
                ScheduleHeaders = c.ScheduleHeaders.Select(e => new Vms.ScheduleHeader
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
                Vouchers = c.Vouchers.Select(d => new Vms.Voucher
                    {
                        Code = d.Code,
                        VoucherTypeName = d.VoucherType.Name,
                        TimeStamp = d.TimeStamp,
                        AmountTransacted = d.AmountTransacted,
                        MemberCode = d.Member,
                        Remark = d.Remark
                    }).ToList()
            }).ToList();
        }
    }

    public class GetAllPurchasedProductByAccountIdQueryValidator : AbstractValidator<GetAllPurchasedProductByAccountIdQuery>
    {
        public GetAllPurchasedProductByAccountIdQueryValidator()
        {
            RuleFor(c => c.AccountId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}