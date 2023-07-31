namespace backend_r.Application.Common.Queries.Member.Vms
{
    public class MemberShipVm
    {
        public int Year { get; set; }
        public List<Data> Datas { get; set; }
    }

    public class Data
    {
        public string Gender { get; set; }
        public int Count { get; set; }
    }
}