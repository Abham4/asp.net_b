namespace backend_r.Domain.Entities
{
    public class Account : EntityBase
    {
        public int ParentAccountId { get; set; }
        public string Description { get; set; }
        public string ControlAccount { get; set; }
        public int AccountProductTypeId { get; set; }
        public AccountProductType AccountProductType { get; set; }
        public List<AccountMap> AccountMaps { get; set; }
        public List<PurchasedProduct> PurchasedProducts { get; set; }

        public Account() {}

        public Account(string code, string description, int accountProductTypeId, List<AccountMap> accountMaps, string createdBy)
        {
            Code = code;
            Description = description;
            AccountProductTypeId = accountProductTypeId;
            AccountMaps = accountMaps;
            CreatedBy = createdBy;
        }
    }
}
