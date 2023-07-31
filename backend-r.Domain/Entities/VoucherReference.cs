namespace backend_r.Domain.Entities
{
    public class VoucherReference : EntityBase
    {
        public int SummaryVoucher { get; set; }
        public int DetailVoucher { get; set; }

        public VoucherReference() {}

        public VoucherReference(int summaryVoucher, int detailVoucher, string createdBy)
        {
            SummaryVoucher = summaryVoucher;
            DetailVoucher = detailVoucher;
            CreatedBy = createdBy;
        }
    }
}