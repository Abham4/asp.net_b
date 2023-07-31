namespace backend_r.Application.Common.Commands.PurchasedProduct
{
    public class CreatePurchaseProductCommand : IRequest<string>
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
        public int ProductWeight => 1;
        public ProductSetup ProductSetup { get; set; }
        public DateTime MaturityDate => DateTime.Now;
        public ScheduleHeader ScheduleHeader { get; set; }
        public bool OverridMembershipMonth { get; set; }
        public bool OverridSaveSharePercentage { get; set; }
    }

    public class ProductSetup
    {
        public double Amount { get; set; }
        public double InterestRate { get; set; }
        public int PaymentCount { get; set; }
        public float PayCycle { get; set; }
        public double PreDepositAmount { get; set; }
        public DateTime PaymentStartDate { get; set; }
    }

    public class ScheduleHeader
    {
        public string Description { get; set; }
        public DateTime ScheduleDate => DateTime.Now;
    }

    public class PurchaseProductCommandHandler : IRequestHandler<CreatePurchaseProductCommand, string>
    {
        private readonly IPurchasedProductRepository _purchasedProductRepo;
        private readonly IProductScheduleRepository _productScheduleRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly IMemberRepository _memberRepo;
        private readonly IAccountMapRepository _accountMapRepo;
        private readonly IScheduleHeaderRepository _scheduleHeaderRepo;
        private readonly IObjectStateRepository _objectStateRepo;
        private readonly IObjectStateDefnRepository _objectStateDefnRepo;
        private readonly IResourceRepository _resourceRepo;
        private readonly IProductRepository _productRepo;
        private readonly IProductRangeRepository _productRangeRepo;
        private readonly IUserRepository _userRepo;
        private readonly IIdDefinitionRepository _idDefinitionRepo;
        private readonly IOrganizationRepository _organizationRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly ILogger<PurchaseProductCommandHandler> _logger;

        public PurchaseProductCommandHandler(IPurchasedProductRepository purchasedProductRepository,
            ILogger<PurchaseProductCommandHandler> logger, IProductScheduleRepository productScheduleRepository,
            IAccountRepository accountRepository, IScheduleHeaderRepository scheduleHeaderRepository,
            IObjectStateRepository objectStateRepository, IObjectStateDefnRepository objectStateDefnRepository,
            IResourceRepository resourceRepository, IProductRepository productRepository,
            IAccountMapRepository accountMapRepository, IMemberRepository memberRepository,
            IProductRangeRepository productRangeRepository, IUserRepository userRepository,
            IIdDefinitionRepository idDefinitionRepository, IOrganizationRepository organizationRepository,
            IVoucherRepository voucherRepository)
        {
            _purchasedProductRepo = purchasedProductRepository;
            _productScheduleRepo = productScheduleRepository;
            _accountRepo = accountRepository;
            _scheduleHeaderRepo = scheduleHeaderRepository;
            _objectStateRepo = objectStateRepository;
            _objectStateDefnRepo = objectStateDefnRepository;
            _resourceRepo = resourceRepository;
            _productRepo = productRepository;
            _accountMapRepo = accountMapRepository;
            _memberRepo = memberRepository;
            _productRangeRepo = productRangeRepository;
            _userRepo = userRepository;
            _idDefinitionRepo = idDefinitionRepository;
            _organizationRepo = organizationRepository;
            _voucherRepo = voucherRepository;
            _logger = logger;
        }

        private int countDigit(int number)
        {
            if (number / 10 == 0)
                return 1;

            return 1 + countDigit(number / 10);
        }

        public async Task<string> Handle(CreatePurchaseProductCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var modifiedInterest = request.ProductSetup.InterestRate / 100;
            var averageDatesInMonth = 30.436875F;

            var productSetup = new Domain.Entities.ProductSetup(request.ProductSetup.Amount,
                request.ProductSetup.PaymentCount, modifiedInterest, request.ProductSetup.PreDepositAmount,
                request.ProductSetup.PaymentStartDate, request.ProductSetup.PayCycle, user.Email);

            var account = await _accountRepo.GetByIdAsync(request.AccountId);
            var product = await _productRepo.GetByIdAsync(request.ProductId);

            int organizationId = 0;
            var member = new Domain.Entities.Member();

            if (product == null)
                throw new DomainException("Product Doesn't exist!");

            if (account == null)
                throw new DomainException("Account doesn't exist!");

            if (account.AccountProductTypeId != product.AccountProductTypeId)
                throw new DomainException("Invalid purchasing!");

            var productSetups = new List<Domain.Entities.ProductSetup> { productSetup };
            var endDate = request.ProductSetup.PaymentStartDate.AddMonths(request.ProductSetup.PaymentCount);

            if (account.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                endDate = request.ProductSetup.PaymentStartDate.AddYears(1);

                if (product.SaveExtention == null)
                    throw new DomainException("Invalid product extension!");

                if (request.ProductSetup.Amount < product.SaveExtention.MinCompulsoryAmount)
                    throw new DomainException("Please use minimum compulsory amount given!");
            }

            else if (account.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                var noofPurchasedShare = _purchasedProductRepo.NoofPrchasedShare(request.AccountId,
                    request.ProductId);

                if (product.ShareExtension == null)
                    throw new DomainException("Invalid product extension!");

                var shareRange = await _productRangeRepo.GetByIdAsync(product.ShareExtension.SharesPerMemberRange);

                if (shareRange == null)
                    throw new DomainException("Invalid share configuration");

                if (shareRange.Max == noofPurchasedShare)
                    throw new DomainException("You've finished allowed number of purchasing this share!");

                if (request.ProductSetup.Amount < product.ShareExtension.NominalPrice || request.ProductSetup.Amount >
                    product.ShareExtension.NominalPrice * shareRange.Max)
                    throw new DomainException("Amount must be with in the range.");
            }

            else
            {
                var reference = await _accountMapRepo.GetRefenceByAccoundIdAndOwner(request.AccountId, "Member");

                organizationId = user.Staff.StaffOrganizations[0].OrganizationId;

                if (!await _organizationRepo.CheckExistence(organizationId))
                    throw new DomainException("User configuration error!");

                if (reference == -1)
                    throw new DomainException("Account configuration error!");

                member = await _memberRepo.GetByIdAsync(reference);

                if (member == null)
                    throw new DomainException("Member account configuration error!");

                var memberShipMonthCount = DateTime.Now.Subtract(member.CreatedDate).Days / averageDatesInMonth;

                if (!request.OverridMembershipMonth)
                    throw new DomainException("Member can't purchase this product with your current Membership Status!");

                var accountMaps = await _accountMapRepo.GetAccountMapByReferenceAndOwner(reference, "Member");
                var shareCount = 0.0;
                var saveCount = 0.0;

                foreach (var item in accountMaps)
                {
                    if (item.Account.AccountProductTypeId == AccountProductType.Sharing.Id)
                        shareCount = await _purchasedProductRepo.TotalPurchasedAmount(item.AccountId);

                    if (item.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                        saveCount = await _purchasedProductRepo.TotalPurchasedAmount(item.AccountId);
                }

                if (product.LoanExtension == null)
                    throw new DomainException("Invalid product extension!");

                if (!request.OverridSaveSharePercentage)
                {
                    var saveSharePercentageToHad = request.ProductSetup.Amount * product.LoanExtension.SaveSharePercentage;
                    var saveShareOnHand = saveCount + shareCount;

                    if (saveSharePercentageToHad > saveShareOnHand)
                        throw new DomainException("Member don't qualify the product SaveShare Percentage!");
                }

                var principalRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.PricipalRange);
                var interestRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.InterestRateRange);
                var repayCyleRange = await _productRangeRepo.GetByIdAsync(product.LoanExtension.RepayCycleRange);

                if (request.ProductSetup.Amount < principalRange.Min || request.ProductSetup.Amount > principalRange.Max)
                    throw new DomainException("Amount must be inclusive in range.");

                if (request.ProductSetup.InterestRate < interestRange.Min || request.ProductSetup.InterestRate >
                    interestRange.Max)
                    throw new DomainException("Interest Rate must be inclusive in range.");

                if (request.ProductSetup.PayCycle < repayCyleRange.Min || request.ProductSetup.PayCycle > repayCyleRange.Max)
                    throw new DomainException("Repay cycle must be inclusive in range.");
            }

            var purchaseProduct = new Domain.Entities.PurchasedProduct(request.AccountId, request.ProductId, productSetups,
                DateTime.Now, request.MaturityDate, endDate, request.ProductWeight, user.Email);

            _logger.LogInformation("-------Purchasing product--------");

            await _purchasedProductRepo.AddAsync(purchaseProduct);
            await _purchasedProductRepo.UnitOfWork.SaveChanges();

            if (account.AccountProductTypeId == AccountProductType.Loan.Id)
            {
                var resource = await _resourceRepo.GetResourceByType(VoucherType.Disbursement.Name);
                var idDefinition = await _idDefinitionRepo.GetByResourseTypeAndOrganizationId(organizationId, resource.Type);
                var value = "";
                var digits = idDefinition.Length - countDigit(idDefinition.NextValue);

                while (digits > 0)
                {
                    value += '0';
                    digits--;
                }

                value += idDefinition.NextValue;

                string code = idDefinition.Prefix + idDefinition.PrefSep + value + idDefinition.SuffSep +
                    idDefinition.Suffix;

                idDefinition.NextValue += 1;

                _idDefinitionRepo.UpdateAsync(idDefinition);
                await _idDefinitionRepo.UnitOfWork.SaveChanges();

                _logger.LogInformation("-------Initial Disbursement--------");

                var voucher = new Domain.Entities.Voucher(code, purchaseProduct.Id, VoucherType.Disbursement.Id,
                    DateTime.Now, request.ProductSetup.Amount, organizationId, member.Code, "Disbursement", user.Email);

                await _voucherRepo.AddAsync(voucher);
                await _voucherRepo.UnitOfWork.SaveChanges();

                var objectStateDefn = await _objectStateDefnRepo.GetObjectStateDefnByResourseTypeAndName("Loan",
                    ObjectStateEnumeration.Active.Name);

                var objectState = new Domain.Entities.ObjectState(objectStateDefn.Id, objectStateDefn.ResourceId,
                    objectStateDefn.Name, DateTime.Now, purchaseProduct.Id, user.Email);

                await _objectStateRepo.AddAsync(objectState);
                await _objectStateRepo.UnitOfWork.SaveChanges();

                purchaseProduct.ProductSetups.OrderBy(c => c.Id).Last().LastObjectState = objectStateDefn.Name;

                _purchasedProductRepo.UpdateAsync(purchaseProduct);
                await _purchasedProductRepo.UnitOfWork.SaveChanges();

                var scheduleHeader = new Domain.Entities.ScheduleHeader(purchaseProduct.Id,
                    request.ScheduleHeader.Description, request.ProductSetup.PaymentStartDate, endDate,
                    request.ScheduleHeader.ScheduleDate, objectStateDefn.Name, user.Email);

                await _scheduleHeaderRepo.AddAsync(scheduleHeader);
                await _scheduleHeaderRepo.UnitOfWork.SaveChanges();

                var startingDate = productSetup.PaymentStartDate;
                var rate = modifiedInterest / 12;
                var due = productSetup.Amount * rate *
                    Math.Pow(1 + rate, productSetup.PaymentCount) /
                    (Math.Pow(1 + rate, productSetup.PaymentCount) - 1);

                due = Math.Round(due, 2);

                var interest = Math.Round(productSetup.Amount * rate, 2);
                var principalDue = Math.Round(due - interest, 2);
                var remain = Math.Round(productSetup.Amount - principalDue, 2);

                await _purchasedProductRepo.Scheduler(AccountProductType.Loan, productSetup.PaymentCount, scheduleHeader.Id,
                    productSetup.PaymentStartDate, principalDue, interest, remain, request.ProductSetup.PayCycle, due, rate);
            }

            if (account.AccountProductTypeId == AccountProductType.Sharing.Id)
            {
                var objectStateDefn = await _objectStateDefnRepo.GetObjectStateDefnByResourseTypeAndName("Share",
                    ObjectStateEnumeration.Active.Name);

                var objectState = new Domain.Entities.ObjectState(objectStateDefn.Id, objectStateDefn.ResourceId,
                    objectStateDefn.Name, DateTime.Now, purchaseProduct.Id, user.Email);

                await _objectStateRepo.AddAsync(objectState);
                await _objectStateRepo.UnitOfWork.SaveChanges();

                purchaseProduct.ProductSetups.OrderBy(c => c.Id).Last().LastObjectState = objectStateDefn.Name;

                _purchasedProductRepo.UpdateAsync(purchaseProduct);
                await _purchasedProductRepo.UnitOfWork.SaveChanges();

                var scheduleHeader = new Domain.Entities.ScheduleHeader(purchaseProduct.Id,
                    request.ScheduleHeader.Description, request.ProductSetup.PaymentStartDate, endDate,
                    request.ScheduleHeader.ScheduleDate, objectStateDefn.Name, user.Email);

                await _scheduleHeaderRepo.AddAsync(scheduleHeader);
                await _scheduleHeaderRepo.UnitOfWork.SaveChanges();

                var startingDate = productSetup.PaymentStartDate;
                var principalDue = Math.Round(productSetup.Amount / productSetup.PaymentCount, 2);
                var remain = Math.Round(productSetup.Amount - Math.Round(productSetup.PaymentCount * principalDue, 2), 2);
                var payCycle = request.ProductSetup.PayCycle;

                await _purchasedProductRepo.Scheduler(AccountProductType.Sharing, productSetup.PaymentCount,
                    scheduleHeader.Id, productSetup.PaymentStartDate, principalDue, 0, remain, payCycle, 0, 0);
            }

            if (account.AccountProductTypeId == AccountProductType.Saving.Id)
            {
                var objectStateDefn = await _objectStateDefnRepo.GetObjectStateDefnByResourseTypeAndName("Save",
                    ObjectStateEnumeration.Active.Name);

                var objectState = new Domain.Entities.ObjectState(objectStateDefn.Id, objectStateDefn.ResourceId,
                    objectStateDefn.Name, DateTime.Now, purchaseProduct.Id, user.Email);

                await _objectStateRepo.AddAsync(objectState);
                await _objectStateRepo.UnitOfWork.SaveChanges();

                purchaseProduct.ProductSetups.OrderBy(c => c.Id).Last().LastObjectState = objectStateDefn.Name;

                _purchasedProductRepo.UpdateAsync(purchaseProduct);
                await _purchasedProductRepo.UnitOfWork.SaveChanges();

                var scheduleHeader = new Domain.Entities.ScheduleHeader(purchaseProduct.Id,
                    request.ScheduleHeader.Description, request.ProductSetup.PaymentStartDate, endDate,
                    request.ScheduleHeader.ScheduleDate, objectStateDefn.Name, user.Email);

                await _scheduleHeaderRepo.AddAsync(scheduleHeader);
                await _scheduleHeaderRepo.UnitOfWork.SaveChanges();

                var payCycle = request.ProductSetup.PayCycle;

                await _purchasedProductRepo.Scheduler(AccountProductType.Saving, 0,
                    scheduleHeader.Id, productSetup.PaymentStartDate, productSetup.Amount, 0, 0,
                    payCycle, 0, 0);
            }

            return "Created";
        }
    }

    public class PurchaseProductCommandValidator : AbstractValidator<CreatePurchaseProductCommand>
    {
        public PurchaseProductCommandValidator()
        {
            RuleFor(c => c.AccountId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.ProductId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.MaturityDate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.ProductSetup.Amount)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.ProductSetup.InterestRate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .GreaterThanOrEqualTo(1.0);

            RuleFor(c => c.ProductSetup.PaymentCount)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .GreaterThanOrEqualTo(1);

            // RuleFor(c => c.ProductSetup.PreDepositAmount)
            //     .NotEmpty().WithMessage("{PropertyName} can't be empty!")
            //     .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.ProductSetup.PaymentStartDate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");

            RuleFor(c => c.ProductSetup.PayCycle)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .GreaterThan(0);

            RuleFor(c => c.ScheduleHeader.Description)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }
    }
}