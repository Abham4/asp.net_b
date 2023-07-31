namespace backend_r.Domain.Interface
{
    public interface IAddressRepository : IBaseRepository<Address>
    {
        Task<List<Address>> GetByReference(int reference);
        Task<List<Address>> GetByReferenceAndOwnerType(int reference, string ownerType);
    }
}
