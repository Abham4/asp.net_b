namespace backend_r.Domain.Entities
{
    public class Spouse : EntityBase
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
        public DateTime RegDate = DateTime.Now;
        public DateTime DOB { get; set; }
        public bool IsActive { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        // Related to Address

        public Spouse() {}

        public Spouse(string title, string firstName, string middleName, string lastName, int genderId, DateTime dob,
            bool isActive, int memberId, string createdBy)
        {
            Title = title;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            GenderId = genderId;
            DOB = dob;
            IsActive = isActive;
            MemberId = memberId;
            CreatedBy = createdBy;
        }
    }
}
