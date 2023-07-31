namespace backend_r.Application.Common.Queries.Staff.Vms
{
    public class OneStaffVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime EmpDate { get; set; }
        public DateTime DOB { get; set; }
        public string LastObjectState { get; set; }
        public OneOrganizationVm Organization { get; set; }
        public List<AddressVm> Addresses { get; set; }
        public List<IdentityVm> Identities { get; set; }
        public string ProfileImg { get; set; }
        public Gender Gender { get; set; }
    }

    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}