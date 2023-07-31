namespace backend_r.Domain.Entities
{
    public class MemberCategory : EntityBase
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public MemberCategory() {}
    }
}
