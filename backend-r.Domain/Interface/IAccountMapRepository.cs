namespace backend_r.Domain.Interface
{
    public interface IAccountMapRepository : IBaseRepository<AccountMap>
    {
        Task<List<AccountMap>> GetAccountMapByReferenceAndOwner(int reference, string owner);
        Task<int> GetRefenceByAccoundIdAndOwner(int accountId, string owner);
    }
}