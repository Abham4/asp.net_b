namespace backend_r.Domain.Entities
{
    public class Journal : EntityBase
    {
        public int VoucherId { get; set; }
        public Voucher Voucher { get; set; }
        public string Account { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        
        public Journal() {}
    }
}