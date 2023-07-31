namespace backend_r.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public string Code { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<RoleClaim> RoleClaims { get; set; }
        public string CreatedBy { get; set; }

        public Role() {}

        public Role(string name, string normalizedName, List<RoleClaim> roleClaims, string createdBy)
        {
            Name = name;
            NormalizedName = normalizedName;
            RoleClaims = roleClaims;
            CreatedBy = createdBy;
        }

        public Role(int id, string name, string normalizedName, DateTime createdDate, string createdBy)
        {
            Id = id;
            Name = name;
            NormalizedName = normalizedName;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
        }
    }
}