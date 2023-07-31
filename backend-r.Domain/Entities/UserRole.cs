namespace backend_r.Domain.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public User User { get; set; }
        public Role Role { get; set; }
        public string Code { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public UserRole() {}

        public UserRole(int roleId)
        {
            RoleId = roleId;
        }

        public UserRole(int userId, int roleId, DateTime createdDate)
        {
            UserId = userId;
            RoleId = RoleId;
            CreatedDate = createdDate;
        }
    }
}