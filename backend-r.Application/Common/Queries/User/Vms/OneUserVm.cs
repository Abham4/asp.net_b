namespace backend_r.Application.Common.Queries.User.Vms
{
    public class OneUserVm
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<UserRole> Roles { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public OneStaffVm Staff { get; set; }
    }

    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<RoleClaimsVm> RoleClaims { get; set; }
    }
}