namespace backend_r.Domain.Entities
{
    public class IdDefinition : EntityBase
    {
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public int? OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int Length { get; set; }
        public string PrefSep { get; set; }
        public string SuffSep { get; set; }
        public int NextValue { get; set; }

        public IdDefinition() {}

        public IdDefinition(int id, int resourceId, int organizationId, string prefix, string prefSep, string suffSep,
            string suffix, int nextValue, DateTime createdDate, int length, string createdBy)
        {
            Id = id;
            ResourceId = resourceId;
            OrganizationId = organizationId;
            Prefix = prefix;
            PrefSep = prefSep;
            SuffSep = suffSep;
            Suffix = suffix;
            NextValue = nextValue;
            CreatedDate = createdDate;
            Length = length;
            CreatedBy = createdBy;
        }

        public IdDefinition(int resourceId, string prefix, string suffix, string prefSep, string suffSep, int nextValue,
            int length, string createdBy)
        {
            ResourceId = resourceId;
            Prefix = prefix;
            Suffix = suffix;
            PrefSep = prefSep;
            SuffSep = suffSep;
            NextValue = nextValue;
            Length = length;
            CreatedBy = createdBy;
        }
    }
}