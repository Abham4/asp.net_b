namespace backend_r.Domain.Entities
{
    public class Category : EntityBase
    {
        public int Type { get; set; }
        public string Description { get; set; }

        public Category() {}
    }
}
