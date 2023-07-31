namespace backend_r.Domain.Interface
{
    public interface IUserRepository
    {
        Task<string> Register(User model);
        Task<string> Modify(User model);
        Task<string> Reset(int id, string password);
        Task<string> Login(string email, string password);
        Task<string> Logout(User user);
        Task<User> GetAuthenticatedUser();
        Task<User> GetUser(int id);
        Task<IReadOnlyList<User>> GetUsers();
        Task<User> GetUserByEmailorPhone(string emailOrPhone);
        Task<bool> CheckExistence(int id);
        IUnitOfWork UnitOfWork { get; }
        List<string> DefaultPermission();
    }
}