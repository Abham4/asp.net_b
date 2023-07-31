namespace backend_r.Application.Common.Queries.Product.Vms
{
    public class OneProductVm
    {
        public int Id { get; set; }
        public string ProductTypeName { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ClosedDate { get; set; }
        public LoanExtension LoanExtension { get; set; }
        public SaveExtension SaveExtension { get; set; }
        public ShareExtension ShareExtension { get; set; }
    }

    public class LoanExtension
    {
        public int NoOfRepayment { get; set; }
        public float PricipalMin { get; set; }
        public float PricipalDefault { get; set; }
        public float PricipalMax { get; set; }
        public float RepaymentMin { get; set; }
        public float RepaymentDefault { get; set; }
        public float RepaymentMax { get; set; }
        public float InterestRateMin { get; set; }
        public float InterestRateDefault { get; set; }
        public float InterestRateMax { get; set; }
        public float RepayCycleMin { get; set; }
        public float RepayCycleDefault { get; set; }
        public float RepayCycleMax { get; set; }
        public int MemebershipMonth { get; set; }
        public float SaveSharePercentage { get; set; }
    }

    public class SaveExtension
    {
        public double MinOpeningBalance { get; set; }
        public double MinRequiredBalance { get; set; }
        public double MinCompulsoryAmount { get; set; }
        public float PayCycle { get; set; }
        // public double PenalityRate { get; set; }
        // public double PenalityAmount { get; set; }
        public double InterestRate { get; set; }
        public double InterestTaxRate { get; set; }
    }

    public class ShareExtension
    {
        public int TotalShareCount { get; set; }
        public double NominalPrice { get; set; }
        public int SharesToBeIssued { get; set; }
        public double CapitalValue { get; set; }
        public float SharesPerMemberMin { get; set; }
        public float SharesPerMemberDefault { get; set; }
        public float SharesPerMemberMax { get; set; }
    }
}