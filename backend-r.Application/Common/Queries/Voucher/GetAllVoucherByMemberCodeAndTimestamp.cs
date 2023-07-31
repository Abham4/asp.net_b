namespace backend_r.Application.Common.Queries.Voucher
{
    public class GetAllVoucherByMemberCodeAndTimestamp : IRequest<List<AllVoucher>>
    {
        public int ClosingPeriodId { get; set; }
        public int MemberId { get; set; }
    }

    public class GetAllVoucherByMemberCodeAndTimestampHandler : IRequestHandler<GetAllVoucherByMemberCodeAndTimestamp,
        List<AllVoucher>>
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly ILogger<GetAllVoucherByMemberCodeAndTimestampHandler> _logger;

        public GetAllVoucherByMemberCodeAndTimestampHandler(IVoucherRepository voucherRepository,
            ILogger<GetAllVoucherByMemberCodeAndTimestampHandler> logger)
        {
            _logger = logger;
            _voucherRepo = voucherRepository;
        }
        
        public async Task<List<AllVoucher>> Handle(GetAllVoucherByMemberCodeAndTimestamp request,
            CancellationToken cancellationToken)
        {
            var vouchers = await _voucherRepo.GetVoucherByMemberCodeAndClosingPeriodId(request.ClosingPeriodId,
                request.MemberId);

            _logger.LogInformation("-------Getting voucher by member code and closing period-----------");

            return vouchers.Select(c => new AllVoucher
            {
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

    public class GetAllVoucherByMemberCodeAndTimestampValidator : AbstractValidator<GetAllVoucherByMemberCodeAndTimestamp>
    {
        public GetAllVoucherByMemberCodeAndTimestampValidator()
        {
            RuleFor(c => c.ClosingPeriodId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!")
                .InclusiveBetween(1, 3);

            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("{PropertyName} can't be empty!")
                .NotNull().WithMessage("{PropertyName} can't be null!");
        }
    }
}