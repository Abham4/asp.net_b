namespace backend_r.Application.Common.Queries.VoucherReference
{
    public class GetAllVoucherReferences : IRequest<IEnumerable<AllVoucherReference>> {}

    public class GetAllVoucherReferencesHandler : IRequestHandler<GetAllVoucherReferences, IEnumerable<AllVoucherReference>
    >
    {
        private readonly IVoucherReferenceRepository _voucherReferenceRepo;
        private readonly IVoucherRepository _voucherRepo;
        private readonly ILogger<GetAllVoucherReferencesHandler> _logger;

        public GetAllVoucherReferencesHandler(IVoucherReferenceRepository voucherReferenceRepository,
            IVoucherRepository voucherRepository, ILogger<GetAllVoucherReferencesHandler> logger)
        {
            _voucherReferenceRepo = voucherReferenceRepository;
            _voucherRepo = voucherRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<AllVoucherReference>> Handle(GetAllVoucherReferences request,
            CancellationToken cancellationToken)
        {
            var voucherReferences = await _voucherReferenceRepo.GetAllAsync();

            _logger.LogInformation("----------Listing All Refered Voucher-----------");

            if(voucherReferences == null)
                return null;

            var detailVouchers = new List<Domain.Entities.Voucher>();
            var summaryVoucherIds = new HashSet<int>();
            var allVoucherReference = new List<AllVoucherReference>();

            foreach (var voucherReference in voucherReferences)
            {
                summaryVoucherIds.Add(voucherReference.SummaryVoucher);
            }

            foreach (var summaryVoucherId in summaryVoucherIds)
            {
                var summaryVoucher = await _voucherRepo.GetByIdAsync(summaryVoucherId);

                var detailVoucher = await _voucherReferenceRepo.GetVoucherReferencesBySummaryVoucher(summaryVoucherId);
                
                foreach (var dvoucher in detailVoucher)
                {
                    var voucher = await _voucherRepo.GetByIdAsync(dvoucher.DetailVoucher);

                    if(voucher != null)
                        detailVouchers.Add(voucher);
                }
                
                if(summaryVoucher != null)
                    allVoucherReference.Add(new AllVoucherReference{
                        Id = summaryVoucher.Id,
                        Code = summaryVoucher.Code,
                        VoucherTypeName = summaryVoucher.VoucherType.Name,
                        TimeStamp = summaryVoucher.TimeStamp,
                        AmountTransacted = summaryVoucher.AmountTransacted,
                        MemberCode = summaryVoucher.Member,
                        OrganizationName = summaryVoucher.Organization.Name,
                        Remark = summaryVoucher.Remark,
                        DetailVouchers = detailVouchers != null ? detailVouchers.Select(c => new PurchasedProduct.Vms.Voucher{
                            Id = c.Id,
                            Code = c.Code,
                            VoucherTypeName = c.VoucherType.Name,
                            TimeStamp = c.TimeStamp,
                            AmountTransacted = c.AmountTransacted,
                            MemberCode = c.Member,
                            OrganizationName = c.Organization.Name,
                            Remark = c.Remark
                        }).ToList() : null
                    });

                detailVouchers.Clear();
            }

            return allVoucherReference;
        }
    }
}