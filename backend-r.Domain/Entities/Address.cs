namespace backend_r.Domain.Entities
{
    public class Address : EntityBase
    {
        public string OwnerType { get; set; }
        public int Reference { get; set; }
        public string AddressType { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }

        public Address() {}
        
        public Address(int id, string ownerType, int reference, string addressType, string attribute, string value,
            DateTime createdDate, string createdBy)
        {
            Id = id;
            OwnerType = ownerType;
            Reference = reference;
            AddressType = addressType;
            Attribute = attribute;
            Value = value;
            CreatedDate = createdDate;
            CreatedBy = createdBy;
        }

        public Address(string ownerType, string addressType, string value, string attribute, int reference, string createdBy)
        {
            OwnerType = ownerType;
            AddressType = addressType;
            Value = value;
            Attribute = attribute;
            Reference = reference;
            CreatedBy = createdBy;
        }
    }
}