namespace backend_r.Domain.Entities
{
    public class Period : EntityBase
    {
        public int Type { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Parent { get; set; }

        public Period() {}
    }
}