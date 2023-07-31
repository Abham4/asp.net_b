namespace backend_r.Domain.Enumerations
{
    public class OrganizationType : Enumeration
    {
        public static OrganizationType Root = new OrganizationType(1, nameof(Root).ToLowerInvariant());
        public static OrganizationType Area = new OrganizationType(2, nameof(Area).ToLowerInvariant());
        public static OrganizationType Branch = new OrganizationType(3, nameof(Branch).ToLowerInvariant());
        public static OrganizationType Sub_Branch = new OrganizationType(4, nameof(Sub_Branch).ToLowerInvariant());
        public static IEnumerable<OrganizationType> List() => new [] { Root, Area, Branch, Sub_Branch };

        private OrganizationType() {}

        private OrganizationType(int id, string name) : base(id, name) {}
    }
}