namespace backend_r.Application.Common.Queries.IdDefinition.Vms
{
    public class IdDefinitionVm
    {
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int Length { get; set; }
        public string PrefSep { get; set; }
        public string SuffSep { get; set; }
        public int NextValue { get; set; }
    }
}