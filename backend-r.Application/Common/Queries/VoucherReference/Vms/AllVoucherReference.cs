namespace backend_r.Application.Common.Queries.VoucherReference.Vms
{
    public class AllVoucherReference
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string VoucherTypeName { get; set; }
        public DateTime TimeStamp { get; set; }
        public double AmountTransacted { get; set; }
        public string MemberCode { get; set; }
        public string OrganizationName { get; set; }
        public string Remark { get; set; }
        public List<PurchasedProduct.Vms.Voucher> DetailVouchers { get; set; }
    }
}