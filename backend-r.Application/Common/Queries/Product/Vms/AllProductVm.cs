namespace backend_r.Application.Common.Queries.Product.Vms
{
    public class AllProductVm
    {
        public int Id { get; set; }
        public string ProductTypeName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosedDate { get; set; }
    }
}