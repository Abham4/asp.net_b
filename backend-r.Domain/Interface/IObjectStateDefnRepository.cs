namespace backend_r.Domain.Interface
{
    public interface IObjectStateDefnRepository : IBaseRepository<ObjectStateDefn>
    {
        Task<ObjectStateDefn> GetObjectStateDefnByResourseTypeAndName(string resourceType, string name);
    }
}