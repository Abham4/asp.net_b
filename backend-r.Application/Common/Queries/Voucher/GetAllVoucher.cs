namespace backend_r.Application.Common.Queries.Voucher
{
    public class GetAllVoucher : IRequest<IEnumerable<AllVoucher>> {}

    public class GetAllVoucherHandler : IRequestHandler<GetAllVoucher, IEnumerable<AllVoucher>>
    {
        private readonly IVoucherRepository _voucherRepo;
        private readonly ILogger<GetAllVoucherHandler> _logger;

        public GetAllVoucherHandler(IVoucherRepository voucherRepository, ILogger<GetAllVoucherHandler> logger)
        {
            _voucherRepo = voucherRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllVoucher>> Handle(GetAllVoucher request, CancellationToken cancellationToken)
        {
            var vouchers = await _voucherRepo.GetAllAsync();

            _logger.LogInformation("-------------Listing All Transactions-----------");

            return vouchers.Select(e => new AllVoucher{
                Id = e.Id,
                Code = e.Code,
                VoucherTypeName = e.VoucherType.Name,
                TimeStamp = e.TimeStamp,
                AmountTransacted = e.AmountTransacted,
                MemberCode = e.Member,
                OrganizationName = e.Organization.Name,
                Remark = e.Remark
            });
        }
    }
}