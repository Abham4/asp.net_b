namespace backend_r.Domain.Entities
{
    public class Occupation : EntityBase
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public int WorkTypeId { get; set; }
        public WorkType WorkType { get; set; }
        public string Position { get; set; }
        public double Income { get; set; }
        public bool IsActive { get; set; }

        // Related to Address

        public Occupation() {}
    }
}