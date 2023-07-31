namespace backend_r.Application.Common.Queries.Member.Vms
{
    public class OneMemberVm
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
        public DateTime DOB { get; set; }
        public string ProfileImg { get; set; }
        public string Signature { get; set; }
        public OneOrganizationVm Organization { get; set;}
        public Gender Gender { get; set;}
        public List<AddressVm> Addresses { get; set; }
        public List<IdentityVm> Identities { get; set; }
        public List<OccupationVm> Occupations { get; set; }
        public List<EducationVm> Educations { get; set; }
        public List<SpouseVm> Spouses { get; set; }
        public List<GuardianVm> Guardians { get; set; }
        public List<AccountMap> AccountMaps { get; set; }
    }

    public class AccountMap
    {
        public List<Account> Accounts { get; set; }
    }

    public class Account
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string ControlAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AccountProductTypeName { get; set; }
    }

    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}