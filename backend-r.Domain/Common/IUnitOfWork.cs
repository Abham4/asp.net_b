namespace backend_r.Domain.Common
{
    public interface IUnitOfWork
    {
        bool IsInTransaction { get; }
        Task SaveChanges();
        Task SaveChanges(SaveOptions saveOptions);
        Task BeginTransaction();
        Task BeginTransaction(IsolationLevel isolationLevel);
        Task RollBackTransaction();
        Task CommitTransaction();
    }
}