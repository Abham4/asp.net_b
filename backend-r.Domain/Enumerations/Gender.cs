namespace backend_r.Domain.Enumerations
{
    public class Gender : Enumeration
    {
        public static Gender Male = new Gender(1, nameof(Male).ToLowerInvariant());
        public static Gender Female = new Gender(2, nameof(Female).ToLowerInvariant());
        public static Gender Not_Specifed = new Gender(3, nameof(Not_Specifed).ToLowerInvariant());
        public static IEnumerable<Gender> List() => new [] { Male, Female, Not_Specifed };

        private Gender() {}

        private Gender(int id, string name) : base(id, name) {}
    }
}