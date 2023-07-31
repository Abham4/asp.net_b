namespace backend_r.Domain.Entities
{
    public class Guardian : EntityBase
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Type { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        // Related to Attachment, Address and Identity

        public Guardian() {}

        public Guardian(string firstName, string middleName, string lastName, int memberId, string createdBy)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            MemberId = memberId;
            CreatedBy = createdBy;
        }
    }
}
