namespace backend_r.Application.Common.Commands.PurchasedProduct
{
    public class UpdatePurchasedProductCommand : IRequest<string>
    {
        public int Id { get; set; }
        public int ProductWeight { get; set; }
        public ProductSetup ProductSetup { get; set; }
        public DateTime MaturityDate => DateTime.Now;
        public ScheduleHeader ScheduleHeader { get; set; }
    }

    public class UpdatePurchasedProductCommandHandler : IRequestHandler<UpdatePurchasedProductCommand, string>
    {
        private readonly IPurchasedProductRepository _purchasedProductRepo;
        private readonly IProductScheduleRepository _productScheduleRepo;
        private readonly IScheduleHeaderRepository _scheduleHeaderRepo;
        private readonly IObjectStateRepository _objectStateRepo;
        private readonly IObjectStateDefnRepository _objectStateDefnRepo;
        private readonly IResourceRepository _resourceRepo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<PurchaseProductCommandHandler> _logger;

        public UpdatePurchasedProductCommandHandler(IPurchasedProductRepository purchasedProductRepository,
            ILogger<PurchaseProductCommandHandler> logger, IProductScheduleRepository productScheduleRepository,
            IScheduleHeaderRepository scheduleHeaderRepository, IObjectStateRepository objectStateRepository,
            IObjectStateDefnRepository objectStateDefnRepository, IResourceRepository resourceRepository,
            IUserRepository userRepository)
        {
            _purchasedProductRepo = purchasedProductRepository;
            _productScheduleRepo = productScheduleRepository;
            _scheduleHeaderRepo = scheduleHeaderRepository;
            _objectStateRepo = objectStateRepository;
            _objectStateDefnRepo = objectStateDefnRepository;
            _resourceRepo = resourceRepository;
            _logger = logger;
            _userRepo = userRepository;
        }

        public async Task<string> Handle(UpdatePurchasedProductCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var purchasedProduct = await _purchasedProductRepo.GetByIdAsync(request.Id);

            if (purchasedProduct == null)
                return null;

            if (purchasedProduct.Account.AccountProductTypeId == AccountProductType.Saving.Id)
                throw new DomainException("Saving can't be updated 4 now!");

            var modifiedInterest = request.ProductSetup.InterestRate / 100;

            var productSetup = new Domain.Entities.ProductSetup(request.ProductSetup.Amount,
                request.ProductSetup.PaymentCount, modifiedInterest, request.ProductSetup.PreDepositAmount,
                request.ProductSetup.PaymentStartDate, request.ProductSetup.PayCycle, user.Email);

            var endDate = request.ProductSetup.PaymentStartDate.AddMonths(request.ProductSetup.PaymentCount);

            if (purchasedProduct.Account.AccountProductType.Id == AccountProductType.Sharing.Id)
            {
                var objectStateDefn = await _objectStateDefnRepo.GetObjectStateDefnByResourseTypeAndName("Share",
                    ObjectStateEnumeration.Archived.Name);

                var objectState = new Domain.Entities.ObjectState(objectStateDefn.Id, objectStateDefn.ResourceId,
                    objectStateDefn.Name, DateTime.Now, purchasedProduct.Id, user.Email);

                await _objectStateRepo.AddAsync(objectState);
                await _objectStateRepo.UnitOfWork.SaveChanges();

                productSetup.LastObjectState = ObjectStateEnumeration.Active.Name;

                purchasedProduct.ProductSetups.SingleOrDefault(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name).LastObjectState = ObjectStateEnumeration.Archived.Name;
                purchasedProduct.ScheduleHeaders.SingleOrDefault(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name).LastObjectState = ObjectStateEnumeration.Archived.Name;

                purchasedProduct.ProductSetups.Add(productSetup);
                purchasedProduct.ProductWeight = request.ProductWeight;
                purchasedProduct.MaturityDate = request.MaturityDate;

                _logger.LogInformation("---------Updating purchased product--------");

                _purchasedProductRepo.UpdateAsync(purchasedProduct);
                await _purchasedProductRepo.UnitOfWork.SaveChanges();

                var scheduleHeader = new Domain.Entities.ScheduleHeader(purchasedProduct.Id, request.ScheduleHeader.Description,
                    request.ProductSetup.PaymentStartDate, endDate, request.ScheduleHeader.ScheduleDate,
                    ObjectStateEnumeration.Active.Name, user.Email);

                await _scheduleHeaderRepo.AddAsync(scheduleHeader);
                await _scheduleHeaderRepo.UnitOfWork.SaveChanges();

                var startingDate = productSetup.PaymentStartDate;
                var principalDue = Math.Round(productSetup.Amount / productSetup.PaymentCount, 2);
                var remain = Math.Round(productSetup.Amount - Math.Round(principalDue * productSetup.Amount, 2), 2);
                var payCycle = request.ProductSetup.PayCycle * 30;

                _logger.LogInformation("-------Adding schedule for purchased product--------");

                await _purchasedProductRepo.Scheduler(AccountProductType.Sharing, productSetup.PaymentCount,
                    scheduleHeader.Id, productSetup.PaymentStartDate, principalDue, 0, remain, payCycle, 0, 0);
            }

            if (purchasedProduct.Account.AccountProductType.Id == AccountProductType.Loan.Id)
            {
                var objectStateDefn = await _objectStateDefnRepo.GetObjectStateDefnByResourseTypeAndName("Loan",
                ObjectStateEnumeration.Archived.Name);

                var objectState = new Domain.Entities.ObjectState(objectStateDefn.Id, objectStateDefn.ResourceId, objectStateDefn.Name,
                    DateTime.Now, purchasedProduct.Id, user.Email);

                await _objectStateRepo.AddAsync(objectState);
                await _objectStateRepo.UnitOfWork.SaveChanges();

                productSetup.LastObjectState = ObjectStateEnumeration.Active.Name;

                purchasedProduct.ProductSetups.SingleOrDefault(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name).LastObjectState = ObjectStateEnumeration.Archived.Name;
                purchasedProduct.ScheduleHeaders.SingleOrDefault(c => c.LastObjectState ==
                    ObjectStateEnumeration.Active.Name).LastObjectState = ObjectStateEnumeration.Archived.Name;

                purchasedProduct.ProductSetups.Add(productSetup);
                purchasedProduct.ProductWeight = request.ProductWeight;
                purchasedProduct.MaturityDate = request.MaturityDate;

                _logger.LogInformation("---------Updating purchased product---------");

                _purchasedProductRepo.UpdateAsync(purchasedProduct);
                await _purchasedProductRepo.UnitOfWork.SaveChanges();

                var scheduleHeader = new Domain.Entities.ScheduleHeader(purchasedProduct.Id, request.ScheduleHeader.Description,
                    request.ProductSetup.PaymentStartDate, endDate, request.ScheduleHeader.ScheduleDate,
                    ObjectStateEnumeration.Active.Name, user.Email);

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
                var payCycle = request.ProductSetup.PayCycle * 30;

                _logger.LogInformation("-------Adding schedule for purchased product--------");

                await _purchasedProductRepo.Scheduler(AccountProductType.Loan, productSetup.PaymentCount, scheduleHeader.Id,
                    productSetup.PaymentStartDate, principalDue, interest, remain, payCycle, due, rate);
            }

            return "Updated";
        }
    }

    public class UpdatePurchasedProductCommandValidator : AbstractValidator<UpdatePurchasedProductCommand>
    {
        public UpdatePurchasedProductCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ProductWeight)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ProductSetup)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ProductSetup.Amount)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ProductSetup.PreDepositAmount)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ProductSetup.PaymentCount)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .GreaterThanOrEqualTo(6);

            RuleFor(c => c.ProductSetup.PaymentStartDate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ProductSetup.InterestRate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null")
                .LessThanOrEqualTo(20.0)
                .GreaterThanOrEqualTo(1.0);

            RuleFor(c => c.ScheduleHeader)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ScheduleHeader.Description)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");

            RuleFor(c => c.ScheduleHeader.ScheduleDate)
                .NotEmpty().WithMessage("{PropertyName} can't be empty")
                .NotNull().WithMessage("{PropertyName} can't be null");
        }
    }
}