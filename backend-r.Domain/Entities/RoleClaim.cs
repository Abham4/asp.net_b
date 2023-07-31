namespace backend_r.Domain.Entities
{
    public class RoleClaim : IdentityRoleClaim<int>
    {
        public Role Role { get; set; }

        public RoleClaim() {}

        public RoleClaim(int roleId, string value)
        {
            RoleId = roleId;
            ClaimType = "AuthorizedTo";
            ClaimValue = value;
        }

        public RoleClaim(string value)
        {
            ClaimType = "AuthorizedTo";
            ClaimValue = value;
        }
    }
}