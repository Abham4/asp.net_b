namespace backend_r.Application.Common.Queries.Role.Vms
{
    public class OneRoleVm
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Users { get; set; }
        public List<RoleClaimsVm> RoleClaims { get; set; }
    }

    public class RoleClaimsVm
    {
        public string Type = "AuthorizedTo";
        public string Value { get; set; }
    }
}