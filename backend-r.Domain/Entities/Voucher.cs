namespace backend_r.Domain.Entities
{
    public class Voucher : EntityBase
    {
        public int? PurchasedProductId { get; set; }
        public PurchasedProduct PurchasedProduct { get; set; }
        public int VoucherTypeId { get; set; }
        public VoucherType VoucherType { get; set; }
        public int? PeriodId { get; set; }
        public Period Period { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string Member { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Purpose { get; set; }
        public double AmountTransacted { get; set; }
        public int Category { get; set; }
        public bool IsVoid { get; set; }
        public bool IsIssued { get; set; }
        public string LastObjectState { get; set; }

        public Voucher() {}

        public Voucher(bool isVoid, string createdBy)
        {
            IsVoid = isVoid;
            CreatedBy = createdBy;
        }

        public Voucher(int purchasedProductId, double amountTransacted, int organizationId, string remark, string createdBy)
        {
            PurchasedProductId = purchasedProductId;
            AmountTransacted = amountTransacted;
            OrganizationId = organizationId;
            Remark = remark;
            CreatedBy = createdBy;
        }

        public Voucher(string code, int purchasedProductId, int voucherTypeId, DateTime timeStamp, double amountTransacted,
            int organizationId, string memberCode, string remark, string createdBy)
        {
            Code = code;
            PurchasedProductId = purchasedProductId;
            VoucherTypeId = voucherTypeId;
            TimeStamp = timeStamp;
            AmountTransacted = amountTransacted;
            OrganizationId = organizationId;
            Member = memberCode;
            Remark = remark;
            CreatedBy = createdBy;
        }

        public Voucher(string code, int voucherTypeId, DateTime timeStamp, double amountTransacted, int organizationId,
            string createdBy)
        {
            Code = code;
            VoucherTypeId = voucherTypeId;
            TimeStamp = timeStamp;
            AmountTransacted = amountTransacted;
            OrganizationId = organizationId;
            CreatedBy = createdBy;
        }

        public Voucher(string code, int voucherTypeId, DateTime timeStamp, double amountTransacted, int organizationId,
            string memberCode, string createdBy)
        {
            Code = code;
            VoucherTypeId = voucherTypeId;
            TimeStamp = timeStamp;
            AmountTransacted = amountTransacted;
            OrganizationId = organizationId;
            Member = memberCode;
            CreatedBy = createdBy;
        }
    }
}