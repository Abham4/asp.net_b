namespace backend_r.Domain.Enumerations
{
    public class ClosingPeriod : Enumeration
    {
        public static ClosingPeriod Daily = new ClosingPeriod(1, nameof(Daily).ToLowerInvariant());
        public static ClosingPeriod Monthly = new ClosingPeriod(2, nameof(Monthly).ToLowerInvariant());
        public static ClosingPeriod Yearly = new ClosingPeriod(3, nameof(Yearly).ToLowerInvariant());
        public static IEnumerable<ClosingPeriod> List() => new [] { Daily, Monthly, Yearly };

        private ClosingPeriod() {}
        private ClosingPeriod(int id, string name) : base(id, name) {}
    }
}