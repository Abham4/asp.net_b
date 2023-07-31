namespace backend_r.Domain.Entities
{
    public class StaffOrganization : EntityBase
    {
        public DateTime AssignmentDate  { get; set; }
        public bool IsContactPerson { get; set; }
        public bool IsActive { get; set; }
        public int StaffId { get; set; }
        public Staff Staff { get; set; }
        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public StaffOrganization() {}
    }
}
