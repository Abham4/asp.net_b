namespace backend_r.Application.Common.Commands.Product
{
    public class CreateProductCommand : IRequest<string>
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public int AccountProductTypeId { get; set; }
        public LoanExtension LoanExtension { get; set; }
        public SaveExtension SaveExtension { get; set; }
        public ShareExtension ShareExtension { get; set; }
    }

    public class SaveExtension
    {
        // public int DormancyDays { get; set; }
        public double MinOpeningBalance { get; set; }
        public double MinRequiredBalance { get; set; }
        public double MinCompulsoryAmount { get; set; }
        public float PayCycle { get; set; }
        // public int PenalityStartAfter { get; set; }
        // public double PenalityRate { get; set; }
        // public double PenalityAmount { get; set; }
        public double InterestRate { get; set; }
        public double InterestTaxRate { get; set; }
    }

    public class ShareExtension
    {
        public int TotalShareCount { get; set; }
        public double NominalPrice { get; set; }
        // public int SharesToBeIssued { get; set; }
        public double CapitalValue => TotalShareCount * NominalPrice;

        // Related to range
        public int SharesPerMemberMax { get; set; }
        public int SharesPerMemberDefault { get; set; }
        public int SharesPerMemberMin { get; set; }
    }

    public class LoanExtension
    {
        public int NoOfRepayment { get; set; }
        public int MembershipMonth { get; set; }
        // public double PenalityRate { get; set; }
        // public int PenalityStartAfter { get; set; }
        public float SaveSharePercentage { get; set; }

        // Related to range
        public int PricipalRangeMin { get; set; }
        public int PricipalRangeDefault { get; set; }
        public int PricipalRangeMax { get; set; }
        public float RepaymentRangeMin { get; set; }
        public float RepaymentRangeDefault { get; set; }
        public float RepaymentRangeMax { get; set; }
        public float InterestRateRangeMin { get; set; }
        public float InterestRateRangeDefault { get; set; }
        public float InterestRateRangeMax { get; set; }
        public int RepayCycleRangeMin { get; set; }
        public int RepayCycleRangeDefault { get; set; }
        public int RepayCycleRangeMax { get; set; }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductRangeRepository _productRangeRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(IProductRepository productRepository,
            IProductRangeRepository productRangeRepository ,ILogger<CreateProductCommandHandler> logger,
            IUserRepository userRepository)
        {
            _productRepo = productRepository;
            _productRangeRepo = productRangeRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var product = new Domain.Entities.Product();

            if(request.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                var sharesPerMemberRange = new Domain.Entities.ProductRange(RangeType.Shares_Per_Member.Id,
                    request.ShareExtension.SharesPerMemberMax, request.ShareExtension.SharesPerMemberMin,
                    request.ShareExtension.SharesPerMemberDefault, user.Email);

                await _productRangeRepo.AddAsync(sharesPerMemberRange);
                await _productRangeRepo.UnitOfWork.SaveChanges();

                var shareExtension = new Domain.Entities.ShareExtension(request.ShareExtension.TotalShareCount,
                    request.ShareExtension.NominalPrice, request.ShareExtension.CapitalValue, sharesPerMemberRange.Id, user.Email);
                
                product = new Domain.Entities.Product(shareExtension, AccountProductType.Sharing.Id, request.Description, 
                    request.StartDate, request.ClosedDate, user.Email);

                _logger.LogInformation("--------Adding Product--------");
            }

            if(request.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                var saveExtension = new Domain.Entities.SaveExtension(request.SaveExtension.MinOpeningBalance,
                    request.SaveExtension.MinRequiredBalance, request.SaveExtension.MinCompulsoryAmount,
                    request.SaveExtension.PayCycle, request.SaveExtension.InterestRate, request.SaveExtension.InterestTaxRate, user.Email);

                product = new Domain.Entities.Product(saveExtension, AccountProductType.Saving.Id, request.Description,
                    request.StartDate, request.ClosedDate, user.Email);
            }

            if(request.AccountProductTypeId == AccountProductType.Loan.Id)
            {
                var pricipalRange = new Domain.Entities.ProductRange(RangeType.Principal.Id,
                    request.LoanExtension.PricipalRangeMax, request.LoanExtension.PricipalRangeMin,
                    request.LoanExtension.PricipalRangeDefault, user.Email);

                var repaymentRange = new Domain.Entities.ProductRange(RangeType.Repayment.Id,
                    request.LoanExtension.RepaymentRangeMax, request.LoanExtension.RepaymentRangeMin,
                    request.LoanExtension.RepaymentRangeDefault, user.Email);

                var interestRateRange = new Domain.Entities.ProductRange(RangeType.Interest_Rate.Id,
                    request.LoanExtension.InterestRateRangeMax, request.LoanExtension.InterestRateRangeMin,
                    request.LoanExtension.InterestRateRangeDefault, user.Email);

                var repaymentCycleRange = new Domain.Entities.ProductRange(RangeType.Repayment_Cycle.Id,
                    request.LoanExtension.RepayCycleRangeMax, request.LoanExtension.RepayCycleRangeMin,
                    request.LoanExtension.RepayCycleRangeDefault, user.Email);

                var rangeList = new List<ProductRange>{ pricipalRange, repaymentRange, interestRateRange,
                    repaymentCycleRange };

                _logger.LogInformation("--------Adding Ranges--------");
                
                for (int i = 0; i < 4; i++)
                {
                    await _productRangeRepo.AddAsync(rangeList[i]);
                    await _productRangeRepo.UnitOfWork.SaveChanges();
                }

                var loanExtension = new Domain.Entities.LoanExtension(request.LoanExtension.NoOfRepayment, pricipalRange.Id,
                    repaymentRange.Id, interestRateRange.Id, repaymentCycleRange.Id, request.LoanExtension.MembershipMonth,
                    request.LoanExtension.SaveSharePercentage / 100, user.Email);

                product = new Domain.Entities.Product(loanExtension, AccountProductType.Loan.Id, request.Description,
                    request.StartDate, request.ClosedDate, user.Email);

                _logger.LogInformation("--------Adding Product--------");
            }

            await _productRepo.AddAsync(product);
            await _productRepo.UnitOfWork.SaveChanges();
            
            return "Created";
        }
    }

    public class CreateProductCommandValidation : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidation()
        {
            RuleFor(c => c.AccountProductTypeId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3).WithMessage("{PropertyName} with null reference");

            RuleFor(c => c.ClosedDate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
            
            RuleFor(c => c.StartDate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
            
            RuleFor(c => c.Description)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            When(c => c.AccountProductTypeId == 1, () => {
               
                RuleFor(c => c.SaveExtension.InterestRate)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");

                RuleFor(c => c.SaveExtension.InterestTaxRate)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");

                RuleFor(c => c.SaveExtension.MinCompulsoryAmount)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");       

                RuleFor(c => c.SaveExtension.MinOpeningBalance)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");       

                RuleFor(c => c.SaveExtension.MinRequiredBalance)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");   

                RuleFor(c => c.SaveExtension.PayCycle)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .GreaterThan(0);            

                // RuleFor(c => c.SaveExtension.PenalityAmount)
                //     .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                //     .NotNull().WithMessage("{PropertyName} can't be null!");            

                // RuleFor(c => c.SaveExtension.PenalityRate)
                //     .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                //     .NotNull().WithMessage("{PropertyName} can't be null!");
            });

            When(c => c.AccountProductTypeId == 2, () => {
                
                RuleFor(c => c.ShareExtension.TotalShareCount)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
            
                RuleFor(c => c.ShareExtension.NominalPrice)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");

                RuleFor(c => c.ShareExtension.SharesPerMemberMax)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.ShareExtension.SharesPerMemberDefault)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.ShareExtension.SharesPerMemberMin)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
            });

            When(c => c.AccountProductTypeId == 3, () => {
                
                RuleFor(c => c.LoanExtension.NoOfRepayment)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.MembershipMonth)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .GreaterThan(0);
                
                RuleFor(c => c.LoanExtension.SaveSharePercentage)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!")
                    .GreaterThan(0);
            
                RuleFor(c => c.LoanExtension.InterestRateRangeMax)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.InterestRateRangeDefault)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.InterestRateRangeMin)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.PricipalRangeMax)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.PricipalRangeDefault)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.PricipalRangeMin)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.RepayCycleRangeMax)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");

                RuleFor(c => c.LoanExtension.RepayCycleRangeDefault)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.RepayCycleRangeMin)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.RepaymentRangeMax)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.RepaymentRangeDefault)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
                
                RuleFor(c => c.LoanExtension.RepaymentRangeMin)
                    .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                    .NotNull().WithMessage("{PropertyName} can't be null!");
            });
        }
    }
}