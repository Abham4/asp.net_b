namespace backend_r.Application.Common.Queries.Staff.Vms
{
    public class AllStaffVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string GenderName { get; set; }
        public DateTime EmpDate { get; set; }
        public DateTime DOB { get; set; }
        public string LastObjectState { get; set; }
        public string OrganizationName { get; set; }
    }
}