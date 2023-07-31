namespace backend_r.Application.Common.Queries.Organization.Vms
{
    public class AllOrganizationVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public string OrganizationTypeName { get; set; }
        public int ParentId { get; set; }
        public string ParentName { get; set; }
        public bool IsActive { get; set; }
    }
}