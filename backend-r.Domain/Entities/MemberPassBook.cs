namespace backend_r.Domain.Entities
{
    public class MemberPassBook : EntityBase
    {
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string State { get; set; }
        public int PassBookId { get; set; }
        public PassBook PassBook { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public MemberPassBook() {}
    }
}
