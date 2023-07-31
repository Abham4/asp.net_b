namespace backend_r.Application.Common.Queries.PurchasedProduct.Vms
{
    public class OnePurchasedProductVm
    {
        public int Id { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProductSetup> ProductSetups { get; set; }
        public List<Voucher> Vouchers { get; set; }
        public List<ScheduleHeader> ScheduleHeaders { get; set; }
        public Product Product { get; set; }
    }

    public class Product
    {
        public string Description { get; set; }
    }

    public class ProductSetup
    {
        public double Amount { get; set; }
        public int PaymentCount { get; set; }
        public float PayCycle { get; set; }
        public double InterestRate { get; set; }
        public double PreDepositAmount { get; set; }
        public DateTime PaymentStartDate { get; set; }
        public string LastObjectState { get; set; }
    }

    public class Voucher
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string VoucherTypeName { get; set; }
        public DateTime TimeStamp { get; set; }
        public double AmountTransacted { get; set; }
        public string MemberCode { get; set; }
        public string OrganizationName { get; set; }
        public string Remark { get; set; }
    }

    public class ScheduleHeader
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string LastObjectState { get; set; }
        public List<ProductSchedule> ProductSchedules { get; set; }
    }

    public class ProductSchedule
    {
        public DateTime DateExpected { get; set; }
        public double PricipalDue { get; set; }
        public double InterestDue { get; set; }
        public bool Status { get; set; }
    }
}