namespace backend_r.Application.Common.Queries.Member.Vms
{
    public class AllMemberVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PassbookCode { get; set; }
        public string MotherName { get; set; }
        public DateTime RegDate { get; set; }
        public string LastObjectState { get; set; }
        public string GenderName { get; set; }
        public DateTime DOB { get; set; }
        public string ProfileImg { get; set; }
        public string Signature { get; set; }
        public OneOrganizationVm Organization { get; set; }
        public List<AddressVm> Addresses { get; set; }
    }
}