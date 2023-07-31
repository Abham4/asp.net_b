namespace backend_r.Domain.Interface
{
    public interface IIdentityRepository : IBaseRepository<Identity>
    {
        Task<List<Identity>> GetByReferenceAndOwnerType(int reference, string owner);
    }
}
