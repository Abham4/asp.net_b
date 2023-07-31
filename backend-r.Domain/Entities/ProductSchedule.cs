namespace backend_r.Domain.Entities
{
    public class ProductSchedule : EntityBase
    {
        public DateTime DateExpected { get; set; }
        public int ScheduleHeaderId { get; set; }
        public ScheduleHeader ScheduleHeader { get; set; }
        public double PricipalDue { get; set; }
        public double InterestDue { get; set; }
        public bool Status { get; set; }

        public ProductSchedule() {}

        public ProductSchedule(int scheduleHeaderId, DateTime dateExpected, double pricipalDue, double interestDue, 
            bool status, string createdBy)
        {
            ScheduleHeaderId = scheduleHeaderId;
            DateExpected = dateExpected;
            PricipalDue = pricipalDue;
            InterestDue = interestDue;
            Status = status;
            CreatedBy = createdBy;
        }

        public ProductSchedule(int scheduleHeaderId, DateTime dateExpected, double pricipalDue, bool status, string createdBy)
        {
            ScheduleHeaderId = scheduleHeaderId;
            DateExpected = dateExpected;
            PricipalDue = pricipalDue;
            Status = status;
            CreatedBy = createdBy;
        }
    }
}