namespace backend_r.Domain.Enumerations
{
    public class WorkType : Enumeration
    {
        public static WorkType Private = new WorkType(1, nameof(Private).ToLowerInvariant());
        public static WorkType Governmental = new WorkType(2, nameof(Governmental).ToLowerInvariant());
        public static WorkType Non_Governmental = new WorkType(3, nameof(Non_Governmental).ToLowerInvariant());
        public static IEnumerable<WorkType> List() => new [] { Private, Governmental, Non_Governmental };

        private WorkType() {}

        private WorkType(int id, string name) : base(id, name) {}
    }
}