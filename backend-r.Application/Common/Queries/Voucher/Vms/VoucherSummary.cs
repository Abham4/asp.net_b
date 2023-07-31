namespace backend_r.Application.Common.Queries.Voucher.Vms
{
    public class VoucherSummary
    {
        public string IncomeOutcomeType { get; set; }
        public List<Period> Periods { get; set; }
    }

    public class Period
    {
        public string PeriodName { get; set; }
        public double Summary { get; set; }
    }
}