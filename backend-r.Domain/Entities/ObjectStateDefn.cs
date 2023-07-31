namespace backend_r.Domain.Entities
{
    public class ObjectStateDefn : EntityBase
    {
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string Name { get; set; }

        public ObjectStateDefn() {}

        public ObjectStateDefn(int id, int resourceId, string name, string createdBy)
        {
            Id = id;
            ResourceId = resourceId;
            Name = name;
            CreatedBy = createdBy;
        }
    }
}