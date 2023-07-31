namespace backend_r.Application.Common.Queries.PurchasedProduct.Vms
{
    public class AllPurchasedProductVm
    {
        public int Id { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime MaturityDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ProductSetup> ProductSetups { get; set; }
    }
}