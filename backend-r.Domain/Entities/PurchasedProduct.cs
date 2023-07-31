namespace backend_r.Domain.Entities
{
    public class PurchasedProduct : EntityBase
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int ProductWeight { get; set; }
        public List<Voucher> Vouchers { get; set; }
        public List<ProductSetup> ProductSetups { get; set; }
        public List<ScheduleHeader> ScheduleHeaders { get; set; }

        public PurchasedProduct() {}

        public PurchasedProduct(int accountId, int productId, List<ProductSetup> productSetups, DateTime purchasedDate, 
        DateTime maturityDate, DateTime endDate, int productWeight, string createdBy)
        {
            AccountId = accountId;
            ProductId = productId;
            ProductSetups = productSetups;
            PurchasedDate = purchasedDate;
            MaturityDate = maturityDate;
            EndDate = endDate;
            ProductWeight = productWeight;
            CreatedBy = createdBy;
        }

        public PurchasedProduct(int accountId, int productId, List<ProductSetup> productSetups, DateTime purchasedDate, 
        DateTime maturityDate, DateTime endDate, int productWeight, List<ScheduleHeader> scheduleHeaders, string createdBy)
        {
            AccountId = accountId;
            ProductId = productId;
            ProductSetups = productSetups;
            PurchasedDate = purchasedDate;
            MaturityDate = maturityDate;
            EndDate = endDate;
            ProductWeight = productWeight;
            ScheduleHeaders = scheduleHeaders;
            CreatedBy = createdBy;
        }
    }
}