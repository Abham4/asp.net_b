namespace backend_r.Domain.Entities
{
    public class Collateral : EntityBase
    {
        public int PurchasedProductId { get; set; }
        public PurchasedProduct PurchasedProduct { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }

        // Related to Attachement

        public Collateral() {}
    }
}