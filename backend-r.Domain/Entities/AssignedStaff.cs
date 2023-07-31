namespace backend_r.Domain.Entities
{
    public class AssignedStaff : EntityBase
    {
        public string Purpuse { get; set; }
        public DateTime AssignedDate { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public int StaffId { get; set; }
        public Staff Staff { get; set; }

        public AssignedStaff() {}
    }
}
