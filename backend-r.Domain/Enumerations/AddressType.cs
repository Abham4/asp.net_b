namespace backend_r.Domain.Enumerations
{
    public class AddressType : Enumeration
    {
        public static AddressType Resedence = new AddressType(1, nameof(Resedence).ToLowerInvariant());
        public static AddressType Office = new AddressType(2, nameof(Office).ToLowerInvariant());
        public static AddressType Social_Media = new AddressType(3, nameof(Social_Media).ToLowerInvariant());
        public static IEnumerable<AddressType> List() => new [] { Resedence, Office, Social_Media };
        
        private AddressType() {}
        private AddressType(int id, string name) : base(id, name) {}
    }
}