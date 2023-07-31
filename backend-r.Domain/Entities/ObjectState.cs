namespace backend_r.Domain.Entities
{
    public class ObjectState : EntityBase
    {
        public int ObjectStateDefnId { get; set; }
        public ObjectStateDefn ObjectStateDefn { get; set; }
        public int Reference { get; set; }
        public int ResourceId { get; set; }
        public Resource Resource { get; set; }
        public string State { get; set; }
        public DateTime TimeStamp { get; set; }

        public ObjectState() {}

        public ObjectState(int objectStateDefnId, int resourceId, string state, DateTime timeStamp, int reference, string createdBy)
        {
            ObjectStateDefnId = objectStateDefnId;
            ResourceId = resourceId;
            State = state;
            TimeStamp = timeStamp;
            Reference = reference;
            CreatedBy = createdBy;
        }
    }
}