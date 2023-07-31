namespace backend_r.Domain.Common
{
    public abstract class EntityBase
    {
        public int Id { get; protected set; }
        public string Code { get; set; }
        public string Remark { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}