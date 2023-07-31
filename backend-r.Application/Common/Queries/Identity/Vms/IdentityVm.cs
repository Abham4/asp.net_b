namespace backend_r.Application.Common.Queries.Identity.Vms
{
    public class IdentityVm
    {
        public int Id { get; set; }
        public string Owner { get; set; }
        public int Reference { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }
}