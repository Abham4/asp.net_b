namespace backend_r.Domain.Entities
{
    public class Resource : EntityBase
    {
        public string Type { get; set; }

        // Have ?
        public string Description { get; set; }

        public Resource() {}

        public Resource(int id, string type, string description, DateTime createdDate, string createdBy)
        {
            Id = id;
            Type = type;
            Description = description;
            CreatedDate = DateTime.Now;
            CreatedBy = createdBy;
        }
    }
}