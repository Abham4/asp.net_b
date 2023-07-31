namespace backend_r.Domain.Interface
{
    public interface IRoleRepository
    {
        Task AddAsync(Role model);
        void Modify(Role model);
        Task<Role> GetByIdAsync(int id);
        Task<Role> GetByName(string roleName);
        Task<IReadOnlyList<Role>> GetAllAsync();
        IUnitOfWork UnitOfWork { get; }
        Task<bool> CheckExistence(int id);
        List<string> DefaultPermission();
    }
}