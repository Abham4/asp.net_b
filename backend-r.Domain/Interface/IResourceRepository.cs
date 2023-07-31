namespace backend_r.Domain.Interface
{
    public interface IResourceRepository : IBaseRepository<Resource>
    {
        Task<Resource> GetResourceByType(string type);
    }
}
