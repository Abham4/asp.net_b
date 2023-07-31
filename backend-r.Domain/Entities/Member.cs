namespace backend_r.Domain.Entities
{
    public class Member : EntityBase
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string MotherName { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime DOB { get; set; }
        public string ProfileImg { get; set; }
        public string Signature { get; set; }
        public string LastObjectState { get; set; }
        public List<MemberOrganization> MemberOrganizations { get; set; }
        public List<Spouse> Spouses { get; set; }
        public List<Education> Educations { get; set; }
        public List<Occupation> Occupations { get; set; }
        public List<Guardian> Guardians { get; set; }
        public List<MemberPassBook> MemberPassBooks { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }

        // Related to Attachment, Address and Identity

        public Member() {}

        public Member(string title, string firstName, string middleName, string lastName, string motherName, int genderId, 
            DateTime dob, List<MemberOrganization> memberOrganizations, List<Spouse> spouses, List<Education> educations,
            List<Occupation> occupations, List<MemberPassBook> memberPassBooks, string createdBy)
        {
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            MotherName = motherName;
            GenderId = genderId;
            DOB = dob;
            MemberOrganizations = memberOrganizations;
            Spouses = spouses;
            Educations = educations;
            Occupations = occupations;
            MemberPassBooks = memberPassBooks;
            CreatedBy = createdBy;
        }

        public Member(string title, string firstName, string middleName, string lastName, string motherName, int genderId, 
            DateTime dob, List<MemberOrganization> memberOrganizations, List<Guardian> guardians,
            List<Education> educations, List<Occupation> occupations, List<MemberPassBook> memberPassBooks, string createdBy)
        {
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            MotherName = motherName;
            GenderId = genderId;
            DOB = dob;
            MemberOrganizations = memberOrganizations;
            Guardians = guardians;
            Educations = educations;
            Occupations = occupations;
            MemberPassBooks = memberPassBooks;
            CreatedBy = createdBy;
        }
    }
}
