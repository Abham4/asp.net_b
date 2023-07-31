namespace backend_r.Domain.Entities
{
    public class ProductSetup : EntityBase
    {
        public int PurchasedProductId { get; set; }
        public PurchasedProduct PurchasedProduct { get; set; }
        public double Amount { get; set; }
        public int PaymentCount { get; set; }
        public double InterestRate { get; set; }
        public float PayCycle { get; set; }
        public double PreDepositAmount { get; set; }
        public DateTime PaymentStartDate { get; set; }

        // Related to ObjectState
        public string LastObjectState { get; set; }

        public ProductSetup()
        {}

        public ProductSetup(double amount, int paymentCount, double interestRate, double preDepositAmount, 
            DateTime paymentStartDate, float payCycle, string createdBy)
        {
            Amount = amount;
            PaymentCount = paymentCount;
            InterestRate = interestRate;
            PreDepositAmount = preDepositAmount;
            PaymentStartDate = paymentStartDate;
            PayCycle = payCycle;
            CreatedBy = createdBy;
        }
    }
}