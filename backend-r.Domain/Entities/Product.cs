namespace backend_r.Domain.Entities
{
    public class Product : EntityBase
    {
        public LoanExtension LoanExtension { get; set; }
        public SaveExtension SaveExtention { get; set; }
        public ShareExtension ShareExtension { get; set; }
        public int AccountProductTypeId { get; set; }
        public AccountProductType AccountProductType { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public string Installment { get; set; }
        public List<PurchasedProduct> PurchasedProducts { get; set; }

        public Product() {}

        public Product(LoanExtension loanExtension, int accountProductTypeId, string description, DateTime startDate, 
            DateTime closedDate, string createdBy)
        {
            LoanExtension = loanExtension;
            AccountProductTypeId = accountProductTypeId;
            Description = description;
            StartDate = startDate;
            ClosedDate = closedDate;
            CreatedBy = createdBy;
        }

        public Product(SaveExtension saveExtension, int accountProductTypeId, string description, DateTime startDate, 
            DateTime closedDate, string createdBy)
        {
            SaveExtention = saveExtension;
            AccountProductTypeId = accountProductTypeId;
            Description = description;
            StartDate = startDate;
            ClosedDate = closedDate;
            CreatedBy = createdBy;
        }

        public Product(ShareExtension shareExtension, int accountProductTypeId, string description, DateTime startDate, 
            DateTime closedDate, string createdBy)
        {
            ShareExtension = shareExtension;
            AccountProductTypeId = accountProductTypeId;
            Description = description;
            StartDate = startDate;
            ClosedDate = closedDate;
            CreatedBy = createdBy;
        }
    }
}