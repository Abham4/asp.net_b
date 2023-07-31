namespace backend_r.Domain.Entities
{
    public class User : IdentityUser<int>
    {
        public string Code { get; set; }
        public string Remark { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public Staff Staff { get; set; }
        public Member Member { get; set; }
        public string CreatedBy { get; set; }
        public int LoggedCount { get; set; }
        public bool IsLogged { get; set; }

        public User() {}

        public User(int id, string email, string phoneNumber, List<UserRole> userRoles, string password, string createdBy)
        {
            Id = id;
            Email = email;
            UserName = email;
            PhoneNumber = phoneNumber;
            LastModifiedDate = DateTime.Now;
            UserRoles = userRoles;
            PasswordHash = password;
            CreatedBy = createdBy;
        }

        public User(int id, string password)
        {
            Id = id;
            PasswordHash = password;
        }

        public User(string email, string securityStamp, string userName, string phoneNumber,
            string password, List<UserRole> userRoles, Staff staff, string createdBy)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            SecurityStamp = securityStamp;
            UserName = userName;
            PasswordHash = password;
            UserRoles = userRoles;
            Staff = staff;
            CreatedBy = createdBy;
        }

        public User(string email, string securityStamp, string userName, string phoneNumber,
            string password, Member member, string createdBy)
        {
            Email = email;
            PhoneNumber = phoneNumber;
            SecurityStamp = securityStamp;
            UserName = userName;
            PasswordHash = password;
            Member = member;
            CreatedBy = createdBy;
        }

        public User(int id, string email, string normalizedEmail, string phoneNumber, string userName, string
            normalizedUserName, DateTime createdDate, Staff staff, string createdBy, string securityStamp)
        {
            Id = id;
            Email = email;
            NormalizedEmail = normalizedEmail;
            EmailConfirmed = true;
            PhoneNumber = phoneNumber;
            PhoneNumberConfirmed = true;
            UserName = userName;
            NormalizedUserName = normalizedUserName;
            LockoutEnabled = false;
            CreatedDate = createdDate;
            Staff = staff;
            CreatedBy = createdBy;
            SecurityStamp = securityStamp;
        }
    }
}