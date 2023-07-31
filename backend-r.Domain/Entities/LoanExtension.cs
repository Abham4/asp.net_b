namespace backend_r.Domain.Entities
{
    public class LoanExtension : EntityBase
    {
        public Product Product { get; set; }
        public int NoOfRepayment { get; set; }

        // Related to ProductRange
        public int PricipalRange { get; set; }
        public int RepaymentRange { get; set; }
        public int InterestRateRange { get; set; }
        public int RepayCycleRange { get; set; }
        public int MembershipMonth { get; set; }
        public double PenalityRate { get; set; }
        public int PenalityStartAfter { get; set; }
        public float SaveSharePercentage { get; set; }

        public LoanExtension() {}

        public LoanExtension(int noOfRepayment, int pricipalRange, int repaymentRange, int interestRateRange,
            int repaymentCycleRange, int membershipMonth, float saveSharePercentage, string createdBy)
        {
            NoOfRepayment = noOfRepayment;
            PricipalRange = pricipalRange;
            RepaymentRange = repaymentRange;
            InterestRateRange = interestRateRange;
            RepayCycleRange = repaymentCycleRange;
            MembershipMonth = membershipMonth;
            SaveSharePercentage = saveSharePercentage;
            CreatedBy = createdBy;
        }
    }
}