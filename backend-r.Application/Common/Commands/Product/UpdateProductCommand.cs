namespace backend_r.Application.Common.Commands.Product
{
    public class UpdateProductCommand : IRequest<string>
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public LoanExtension LoanExtension { get; set; }
        public SaveExtension SaveExtension { get; set; }
        public ShareExtension ShareExtension { get; set; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, string>
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductRangeRepository _productRangeRepo;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IProductRepository productRepository, ILogger<UpdateProductCommandHandler>
            logger, IProductRangeRepository productRangeRepository)
        {
            _logger = logger;
            _productRepo = productRepository;
            _productRangeRepo = productRangeRepository;
        }

        public async Task<string> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _productRepo.GetByIdAsync(request.Id);

            if (product == null)
                return null;

            if (product.AccountProductTypeId == AccountProductType.Loan.Id)
            {
                if (product.LoanExtension == null)
                    throw new DomainException("Broken Product!");

                product.Description = request.Description != null ? request.Description : product.Description;
                product.StartDate = request.StartDate != product.StartDate ? request.StartDate : product.StartDate;
                product.ClosedDate = request.ClosedDate != product.ClosedDate ? request.ClosedDate : product.ClosedDate;
                product.LoanExtension.NoOfRepayment = request.LoanExtension.NoOfRepayment != 0 ? request.LoanExtension
                    .NoOfRepayment : product.LoanExtension.NoOfRepayment;
                product.LoanExtension.MembershipMonth = request.LoanExtension.MembershipMonth != 0 ? request
                    .LoanExtension.MembershipMonth : product.LoanExtension.MembershipMonth;
                product.LoanExtension.SaveSharePercentage = request.LoanExtension.SaveSharePercentage != 0 ? request
                    .LoanExtension.SaveSharePercentage : product.LoanExtension.SaveSharePercentage;

                var pricipalRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.PricipalRange);
                var repaymentRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.RepaymentRange);
                var interestRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.InterestRateRange);
                var repayCycleRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.RepayCycleRange);

                if (pricipalRange == null)
                    throw new DomainException("Broken Loan Product!");

                if (repaymentRange == null)
                    throw new DomainException("Broken Loan Product!");

                if (interestRange == null)
                    throw new DomainException("Broken Loan Product!");

                if (repayCycleRange == null)
                    throw new DomainException("Broken Loan Product!");

                pricipalRange.Min = request.LoanExtension.PricipalRangeMin != 0 ? request.LoanExtension.PricipalRangeMin
                    : pricipalRange.Min;
                pricipalRange.DefaultValue = request.LoanExtension.PricipalRangeDefault != 0 ? request.LoanExtension
                    .PricipalRangeDefault : pricipalRange.DefaultValue;
                pricipalRange.Max = request.LoanExtension.PricipalRangeMax != 0 ? request.LoanExtension.PricipalRangeMax
                    : pricipalRange.Max;

                _logger.LogInformation("--------Updating Principal Range-------");

                _productRangeRepo.UpdateAsync(pricipalRange);
                await _productRangeRepo.UnitOfWork.SaveChanges();

                repaymentRange.Min = request.LoanExtension.RepaymentRangeMin != 0 ? request.LoanExtension.RepaymentRangeMin
                    : repaymentRange.Min;
                repaymentRange.DefaultValue = request.LoanExtension.RepaymentRangeDefault != 0 ? request.LoanExtension
                    .RepaymentRangeDefault : repaymentRange.DefaultValue;
                repaymentRange.Max = request.LoanExtension.RepayCycleRangeMax != 0 ? request.LoanExtension
                    .RepaymentRangeMax : repaymentRange.Max;

                _logger.LogInformation("--------Updating Repayment Range-------");

                _productRangeRepo.UpdateAsync(repaymentRange);
                await _productRangeRepo.UnitOfWork.SaveChanges();

                interestRange.Min = request.LoanExtension.InterestRateRangeMin != 0 ? request.LoanExtension
                    .InterestRateRangeMin : interestRange.Min;
                interestRange.DefaultValue = request.LoanExtension.InterestRateRangeDefault != 0 ? request.LoanExtension
                    .InterestRateRangeDefault : interestRange.DefaultValue;
                interestRange.Max = request.LoanExtension.InterestRateRangeMax != 0 ? request.LoanExtension
                    .InterestRateRangeMax : interestRange.Max;

                _logger.LogInformation("--------Updating Interest Range-------");

                _productRangeRepo.UpdateAsync(interestRange);
                await _productRangeRepo.UnitOfWork.SaveChanges();

                repayCycleRange.Min = request.LoanExtension.RepayCycleRangeMin != 0 ? request.LoanExtension
                    .RepayCycleRangeMin : repayCycleRange.Min;
                repayCycleRange.DefaultValue = request.LoanExtension.RepayCycleRangeDefault != 0 ? request.LoanExtension
                    .RepayCycleRangeDefault : repayCycleRange.DefaultValue;
                repayCycleRange.Max = request.LoanExtension.RepayCycleRangeMax != 0 ? request.LoanExtension
                    .RepayCycleRangeMax : repayCycleRange.Max;

                _logger.LogInformation("--------Updating Repay Cycle Range-------");

                _productRangeRepo.UpdateAsync(repayCycleRange);
                await _productRangeRepo.UnitOfWork.SaveChanges();
            }

            if (product.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                if (product.SaveExtention == null)
                    throw new DomainException("Broken Product!");

                product.Description = request.Description != null ? request.Description : product.Description;
                product.StartDate = request.StartDate != product.StartDate ? request.StartDate : product.StartDate;
                product.ClosedDate = request.ClosedDate != product.ClosedDate ? request.ClosedDate : product.ClosedDate;
                product.SaveExtention.MinOpeningBalance = request.SaveExtension.MinOpeningBalance != product.SaveExtention.
                    MinOpeningBalance ? request.SaveExtension.MinOpeningBalance : product.SaveExtention.MinOpeningBalance;
                product.SaveExtention.MinRequiredBalance = request.SaveExtension.MinRequiredBalance != product.SaveExtention.
                    MinRequiredBalance ? request.SaveExtension.MinRequiredBalance : product.SaveExtention.MinRequiredBalance;
                product.SaveExtention.MinCompulsoryAmount = request.SaveExtension.MinCompulsoryAmount != product.SaveExtention.
                    MinCompulsoryAmount ? request.SaveExtension.MinCompulsoryAmount : product.SaveExtention.MinCompulsoryAmount;
                product.SaveExtention.PayCycle = request.SaveExtension.PayCycle != product.SaveExtention.
                    PayCycle ? request.SaveExtension.PayCycle : product.SaveExtention.PayCycle;
                product.SaveExtention.InterestRate = request.SaveExtension.InterestRate != product.SaveExtention.
                    InterestRate ? request.SaveExtension.InterestRate : product.SaveExtention.InterestRate;
                product.SaveExtention.InterestTaxRate = request.SaveExtension.InterestTaxRate != product.SaveExtention.
                    InterestTaxRate ? request.SaveExtension.InterestTaxRate : product.SaveExtention.InterestTaxRate;
            }

            if (product.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                if (product.ShareExtension == null)
                    throw new DomainException("Broken Product!");

                product.Description = request.Description != null ? request.Description : product.Description;
                product.StartDate = request.StartDate != product.StartDate ? request.StartDate : product.StartDate;
                product.ClosedDate = request.ClosedDate != product.ClosedDate ? request.ClosedDate : product.ClosedDate;

                var sharesPerMemberRange = await _productRangeRepo.GetByIdAsync(product.ShareExtension
                    .SharesPerMemberRange);

                sharesPerMemberRange.Min = request.ShareExtension.SharesPerMemberMin != 0 ? request.ShareExtension
                    .SharesPerMemberMin : sharesPerMemberRange.Min;
                sharesPerMemberRange.DefaultValue = request.ShareExtension.SharesPerMemberDefault != 0 ? request
                    .ShareExtension.SharesPerMemberDefault : sharesPerMemberRange.DefaultValue;
                sharesPerMemberRange.Max = request.ShareExtension.SharesPerMemberMax != 0 ? request.ShareExtension
                    .SharesPerMemberMax : sharesPerMemberRange.Max;

                _logger.LogInformation("----------Updating Shares per member range--------");

                _productRangeRepo.UpdateAsync(sharesPerMemberRange);
                await _productRangeRepo.UnitOfWork.SaveChanges();
            }

            _logger.LogInformation("--------Updating product----------");

            _productRepo.UpdateAsync(product);
            await _productRepo.UnitOfWork.SaveChanges();

            return "Updated";
        }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} must not be empty")
                .NotNull().WithMessage("{PropertyName} can'be null");
        }
    }
}