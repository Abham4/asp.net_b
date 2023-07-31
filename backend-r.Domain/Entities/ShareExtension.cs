namespace backend_r.Domain.Entities
{
    public class ShareExtension : EntityBase
    {
        public Product Product { get; set; }
        public int TotalShareCount { get; set; }
        public double NominalPrice { get; set; }
        public int SharesToBeIssued { get; set; }
        public double CapitalValue { get; set; }

        // Related to ProductRange
        public int SharesPerMemberRange { get; set; }

        public ShareExtension() {}

        public ShareExtension(int totalShareCount, double nominalPrice, double capitalValue, int sharesPerMemberRange, string createdBy)
        {
            TotalShareCount = totalShareCount;
            NominalPrice = nominalPrice;
            CapitalValue = capitalValue;
            SharesPerMemberRange = sharesPerMemberRange;
            CreatedBy = createdBy;
        }
    }
}