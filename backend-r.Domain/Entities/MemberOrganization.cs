namespace backend_r.Domain.Entities
{
    public class MemberOrganization : EntityBase
    {
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public MemberOrganization() {}
    }
}
