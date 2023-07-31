namespace backend_r.Application.Common.Queries.Organization.Vms
{
    public class OneOrganizationVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string OrganizationTypeName { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public List<IdDefinitionVm> IdDefinitions { get; set; }
        public List<StaffOrganization> StaffOrganizations { get; set; }
    }

    public class StaffOrganization
    {
        public bool IsActive { get; set; }
        public bool IsContactPerson { get; set; }
        public List<AllStaffVm> Staffs { get; set; }
    }
}