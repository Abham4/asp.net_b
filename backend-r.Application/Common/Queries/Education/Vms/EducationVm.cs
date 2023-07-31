namespace backend_r.Application.Common.Queries.Education.Vms
{
    public class EducationVm
    {
        public int Id { get; set; }
        public string Institution { get; set; }
        public string MemberFullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Level { get; set; }
        public string FieldOfStudy { get; set; }
        public bool IsActive { get; set; }
    }
}