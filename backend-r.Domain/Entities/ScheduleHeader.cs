namespace backend_r.Domain.Entities
{
    public class ScheduleHeader : EntityBase
    {
        public int PurchasedProductId { get; set; }
        public PurchasedProduct PurchasedProduct { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime ScheduleDate { get; set; }

        // Related to ObjectState
        public string LastObjectState { get; set; }
        public List<ProductSchedule> ProductSchedules { get; set; }

        public ScheduleHeader() {}

        public ScheduleHeader(int purchasedProductId, string description, DateTime startDate, DateTime endDate, 
            DateTime scheduleDate, string lastObjectState, string createdBy)
        {
            PurchasedProductId = purchasedProductId;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            ScheduleDate = scheduleDate;
            LastObjectState = lastObjectState;
            CreatedBy = createdBy;
        }
    }
}