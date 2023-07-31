namespace backend_r.Application.Common.Queries.Voucher
{
    public class GetAllVoucherByOrganizationAndTimestamp : IRequest<List<AllVoucher>>
    {
        public int OrganizationId { get; set; }
        public int ClosingPeriodId { get; set; }
    }

    public class GetAllVoucherByOrganizationAndTimestampHandler : IRequestHandler<GetAllVoucherByOrganizationAndTimestamp,
        List<AllVoucher>>
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly ILogger<GetAllVoucherByOrganizationAndTimestampHandler> _logger;

        public GetAllVoucherByOrganizationAndTimestampHandler(IVoucherRepository voucherRepository, 
            ILogger<GetAllVoucherByOrganizationAndTimestampHandler> logger)
        {
            _logger = logger;
            _voucherRepo = voucherRepository;
        }

        public async Task<List<AllVoucher>> Handle(GetAllVoucherByOrganizationAndTimestamp request,
            CancellationToken cancellationToken)
        {
            var vouchers = await _voucherRepo.GetVoucherByOrganizationIdAndClosingPeriodId(request.ClosingPeriodId,
                request.OrganizationId);

            _logger.LogInformation("-------Getting vouchers by organization and closing period----------");

            return vouchers.Select(c => new AllVoucher{
                Code = c.Code,
                VoucherTypeName = c.VoucherType.Name,
                TimeStamp = c.TimeStamp,
                AmountTransacted = c.AmountTransacted,
                MemberCode = c.Member,
                OrganizationName = c.Organization.Name,
                Remark = c.Remark
            }).ToList();
        }
    }

    public class GetAllVoucherByOrganizationAndTimestampValidator : AbstractValidator<GetAllVoucherByOrganizationAndTimestamp>
    {
        public GetAllVoucherByOrganizationAndTimestampValidator()
        {
            RuleFor(c => c.ClosingPeriodId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3);

            RuleFor(c => c.OrganizationId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}