namespace backend_r.Domain.Enumerations
{
    public class    AccountProductType : Enumeration
    {
        public static AccountProductType Saving = new AccountProductType(1, nameof(Saving).ToLowerInvariant());
        public static AccountProductType Sharing = new AccountProductType(2, nameof(Sharing).ToLowerInvariant());
        public static AccountProductType Loan = new AccountProductType(3, nameof(Loan).ToLowerInvariant());
        public static IEnumerable<AccountProductType> List() => new [] { Saving, Sharing, Loan };

        private AccountProductType()
        {
        }

        private AccountProductType(int id, string name) : base(id, name) {}
    }
}