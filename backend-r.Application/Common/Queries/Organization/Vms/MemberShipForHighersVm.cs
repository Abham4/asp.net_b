namespace backend_r.Application.Common.Queries.Organization.Vms
{
    public class MemberShipForHighersVm
    {
        public string Organization { get; set; }
        public List<FullData> FullDatas { get; set; }
    }

    public class FullData
    {
        public int Year { get; set; }
        public List<Data> Datas { get; set; }
    }
}