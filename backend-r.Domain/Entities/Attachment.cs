namespace backend_r.Domain.Entities
{
    public class Attachment : EntityBase
    {
        public string Owner { get; set; }
        public int Reference { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public DateTime CaptureDate { get; set; }

        public Attachment() {}
    }
}
