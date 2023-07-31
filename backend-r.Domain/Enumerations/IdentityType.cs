namespace backend_r.Domain.Enumerations
{
    public class IdentityType : Enumeration
    {
        public static IdentityType Kebele = new IdentityType(1, nameof(Kebele).ToLowerInvariant());
        public static IdentityType Office = new IdentityType(2, nameof(Office).ToLowerInvariant());
        public static IdentityType Passport = new IdentityType(3, nameof(Passport).ToLowerInvariant());
        public static IdentityType Driving_License = new IdentityType(4, nameof(Driving_License).ToLowerInvariant());
        public static IEnumerable<IdentityType> List() => new [] { Kebele, Office, Passport, Driving_License };

        private IdentityType() {}
        private IdentityType(int id, string name) : base(id, name) {}
    }
}