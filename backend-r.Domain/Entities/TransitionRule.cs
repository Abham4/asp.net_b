namespace backend_r.Domain.Entities
{
    public class TransitionRule : EntityBase
    {
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool RequirePWD { get; set; }

        public TransitionRule() {}
    }
}