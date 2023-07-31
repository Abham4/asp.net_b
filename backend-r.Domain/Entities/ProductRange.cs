namespace backend_r.Domain.Entities
{
    public class ProductRange : EntityBase
    {
        public int RangeTypeId { get; set; }
        public RangeType RangeType { get; set; }
        public string Description { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float DefaultValue { get; set; }

        public ProductRange() {}

        public ProductRange(int rangeTypeId, float max, float min, float defaultValue, string createdBy)
        {
            RangeTypeId = rangeTypeId;
            Max = max;
            Min = min;
            DefaultValue = defaultValue;
            CreatedBy = createdBy;
        }
    }
}