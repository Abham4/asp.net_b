namespace backend_r.Domain.Entities
{
    public class Organization : EntityBase
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string  Description { get; set; }
        public int ParentId { get; set; }
        public bool IsActive { get; set; }
        public DateTime OpeningDate { get; set; }
        public int OrganizationTypeId { get; set; }
        public OrganizationType OrganizationType { get; set; }
        public List<IdDefinition> IdDefinitions { get; set; }
        public List<StaffOrganization> StaffOrganizations { get; set; }
        public List<MemberOrganization> MemberOrganizations { get; set; }

        // Related to Address

        public Organization() {}

        public Organization(int id, string name, string description, int organizationTypeId, int parentId, bool isActive,
            DateTime createdDate, string shortName, string createdBy)
        {
            Id = id;
            Name = name;
            Description = description;
            OrganizationTypeId = organizationTypeId;
            ParentId = parentId;
            IsActive = isActive;
            CreatedDate = createdDate;
            ShortName = shortName;
            CreatedBy = createdBy;
        }
        public Organization(string name, string shortName, string description, int organizationTypeId, int parentId,
            bool isActive, List<StaffOrganization> staffOrganizations, List<IdDefinition> idDefinitions, string createdBy)
        {
            Name = name;
            ShortName = shortName;
            Description = description;
            OrganizationTypeId = organizationTypeId;
            ParentId = parentId;
            IsActive = isActive;
            StaffOrganizations = staffOrganizations;
            IdDefinitions = idDefinitions;
            CreatedBy = createdBy;
        }
    }
}
