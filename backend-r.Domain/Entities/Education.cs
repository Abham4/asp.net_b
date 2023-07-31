namespace backend_r.Domain.Entities
{
    public class Education : EntityBase
    {
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public string Institution { get; set; }
        public string Level { get; set; }
        public string FieldOfStudy { get; set; }
        public bool IsActive { get; set; }

        public Education() {}
    }
}