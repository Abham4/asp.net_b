namespace backend_r.Application.Common.Queries.Organization.Vms
{
    public class MemberCarrersForHigersVm
    {
        public string Organization { get; set; }
        public List<MemberCarrersVm> MemberCarrers { get; set; }
    }
}