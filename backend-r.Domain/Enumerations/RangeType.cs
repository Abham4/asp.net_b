namespace backend_r.Domain.Enumerations
{
    public class RangeType : Enumeration
    {
        public static RangeType Principal = new RangeType(1, nameof(Principal).ToLowerInvariant());
        public static RangeType Repayment = new RangeType(2, nameof(Repayment).ToLowerInvariant());
        public static RangeType Interest_Rate = new RangeType(3, nameof(Interest_Rate).ToLowerInvariant());
        public static RangeType Repayment_Cycle = new RangeType(4, nameof(Repayment_Cycle).ToLowerInvariant());
        public static RangeType Shares_Per_Member = new RangeType(5, nameof(Shares_Per_Member).ToLowerInvariant());
        public static IEnumerable<RangeType> List() => new [] { Principal, Repayment, Interest_Rate, Repayment_Cycle,
            Shares_Per_Member };

        private RangeType() {}

        private RangeType(int id, string name) : base(id, name) {}
    }
}