namespace backend_r.Application.Common.Queries.Voucher.Vms
{
    public class VoucherSummaryforHighers
    {
        public string Organization { get; set; }
        public List<VoucherSummary> VoucherSummaries { get; set; }
    }
}