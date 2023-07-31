namespace backend_r.Application.Common.Commands.Voucher
{
    public class CreateVoucherWithdraw : IRequest<IEnumerable<AllVoucher>>
    {
        public int PurchasedProductId { get; set; }
        public double Amount { get; set; }
        public string Remark { get; set; }
    }

    public class CreateVoucherWithdrawHandler : IRequestHandler<CreateVoucherWithdraw, IEnumerable<AllVoucher>>
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly IUserRepository _userRepo;
        private readonly IOrganizationRepository _organizationRepo;
        private readonly ILogger<CreateVoucherWithdrawHandler> _logger;

        public CreateVoucherWithdrawHandler(IVoucherRepository voucherRepository, ILogger<CreateVoucherWithdrawHandler> logger,
            IUserRepository userRepository, IOrganizationRepository organizationRepository)
        {
            _logger = logger;
            _voucherRepo = voucherRepository;
            _userRepo = userRepository;
            _organizationRepo = organizationRepository;
        }

        public async Task<IEnumerable<AllVoucher>> Handle(CreateVoucherWithdraw request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetAuthenticatedUser();
            var organizationId = user.Staff.StaffOrganizations[0].OrganizationId;

            if (!await _organizationRepo.CheckExistence(organizationId))
                throw new DomainException("User configuration error!");

            var vouchers = await _voucherRepo.Withdraw(organizationId, request.PurchasedProductId, request.Amount,
                request.Remark);

            _logger.LogInformation("---------Creating Withdraw Voucher----------");

            return vouchers.Select(c => new AllVoucher
            {
                Code = c.Code,
                VoucherTypeName = c.VoucherType.Name,
                TimeStamp = c.TimeStamp,
                AmountTransacted = c.AmountTransacted,
                MemberCode = c.Member,
                OrganizationName = c.Organization.Name,
                Remark = c.Remark
            });
        }
    }

    public class CreateVoucherWithdrawValidation : AbstractValidator<CreateVoucherWithdraw>
    {
        public CreateVoucherWithdrawValidation()
        {
            RuleFor(c => c.Amount)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .GreaterThan(0);

            RuleFor(c => c.PurchasedProductId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}