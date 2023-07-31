namespace backend_r.Domain.Entities
{
    public class Identity : EntityBase
    {
        public string Owner { get; set; }
        public int Reference { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }

        public Identity() {}

        public Identity(string owner, string type, string description, string number, int reference, string createdBy)
        {
            Owner = owner;
            Type = type;
            Description = description;
            Number = number;
            Reference = reference;
            CreatedBy = createdBy;
        }
    }
}
