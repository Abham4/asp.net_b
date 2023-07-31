namespace backend_r.Domain.Interface
{
    public interface IObjectStateRepository : IBaseRepository<ObjectState>
    {
        Task<string> ToActive(int memberId);
        Task<string> ToTerminate(int memberId);
    }
}
