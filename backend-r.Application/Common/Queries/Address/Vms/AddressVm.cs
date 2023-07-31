namespace backend_r.Application.Common.Queries.Address.Vms
{
    public class AddressVm
    {
        public int Id { get; set; }
        public string OwnerType { get; set; }
        public int Reference { get; set; }
        public string AddressType { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
    }
}