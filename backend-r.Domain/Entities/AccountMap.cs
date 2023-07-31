namespace backend_r.Domain.Entities
{
    public class AccountMap : EntityBase
    {
        public string Owner { get; set; }
        public int Reference { get; set; }
        public int AccountId { get; set; }
        public Account Account { get; set; }

        public AccountMap() {}

        public AccountMap(string owner, int reference, int accountId, string createdBy)
        {
            AccountId = accountId;
            Reference = reference;
            Owner = owner;
            CreatedBy = createdBy;
        }
    }
}
