namespace backend_r.Domain.Entities
{
    public class SaveExtension : EntityBase
    {
        public Product Product { get; set; }
        public int DormancyDays { get; set; }
        public double MinOpeningBalance { get; set; }
        public double MinRequiredBalance { get; set; }
        public double MinCompulsoryAmount { get; set; }
        public float PayCycle { get; set; }
        public int PenalityStartAfter { get; set; }
        public float PenalityRate { get; set; }
        public double PenalityAmount { get; set; }
        public double InterestRate { get; set; }
        public double InterestTaxRate { get; set; }

        public SaveExtension() {}

        public SaveExtension(double minOpeningBalance, double minRequiredBalance, double minCompulsoryAmount, float payCycle, 
            double interestRate, double interestTaxRate, string createdBy)
        {
            MinOpeningBalance = minOpeningBalance;
            MinRequiredBalance = minRequiredBalance;
            MinCompulsoryAmount = minCompulsoryAmount;
            PayCycle = payCycle;
            InterestRate = interestRate;
            InterestTaxRate = interestTaxRate;
            CreatedBy = createdBy;
        }
    }
}