namespace backend_r.Domain.Enumerations
{
    public class ObjectStateEnumeration : Enumeration
    {
        public static ObjectStateEnumeration Active = new ObjectStateEnumeration(1, nameof(Active).ToLowerInvariant());
        public static ObjectStateEnumeration Created = new ObjectStateEnumeration(2, nameof(Created).ToLowerInvariant());
        public static ObjectStateEnumeration Terminated = new ObjectStateEnumeration(3, nameof(Terminated).ToLowerInvariant());
        public static ObjectStateEnumeration Archived = new ObjectStateEnumeration(4, nameof(Archived).ToLowerInvariant());
        public static ObjectStateEnumeration Closed = new ObjectStateEnumeration(5, nameof(Closed).ToLowerInvariant());
        public static IEnumerable<ObjectStateEnumeration> List() => new [] { Active, Created, Terminated, Archived };

        private ObjectStateEnumeration() {}

        private ObjectStateEnumeration(int id, string name) : base(id, name) {}
    }
}