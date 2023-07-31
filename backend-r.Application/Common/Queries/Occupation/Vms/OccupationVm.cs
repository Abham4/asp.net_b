namespace backend_r.Application.Common.Queries.Occupation.Vms
{
    public class OccupationVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public string WorkTypeName { get; set; }
        public double Income { get; set; }
        public bool IsActive { get; set; }
        public List<AddressVm> Addresses { get; set; }
    }
}