namespace backend_r.Domain.Entities
{
    public class Staff : EntityBase
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
        public DateTime EmpDate { get; set; }
        public DateTime DOB { get; set; }
        public string ProfileImg { get; set; }
        public string LastObjectState { get; set; }
        public List<StaffOrganization> StaffOrganizations { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }

        // Related to Attachment, Address and Identity

        public Staff() {}

        public Staff(int id, string firstName, string lastName, string createdBy)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            GenderId = Gender.Not_Specifed.Id;
            CreatedBy = createdBy;
        }

        public Staff(int id, string firstName, string lastName, int organizationId, string createdBy)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            GenderId = Gender.Not_Specifed.Id;
            StaffOrganizations = new List<StaffOrganization>{
                new StaffOrganization{
                    OrganizationId = organizationId
                }
            };
            CreatedBy = createdBy;
        }

        public Staff(string title, string firstName, string middleName, string lastName, int genderId,
            DateTime dob, List<StaffOrganization> staffOrganizations, string profileImg, string createdBy)
        {
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            GenderId = genderId;
            DOB = dob;
            StaffOrganizations = staffOrganizations;
            ProfileImg = profileImg;
            EmpDate = DateTime.Now;
            CreatedBy = createdBy;
        }
    }
}
