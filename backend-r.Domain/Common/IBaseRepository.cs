namespace backend_r.Domain.Common
{
    public interface IBaseRepository <T> where T : EntityBase
    {
        IUnitOfWork UnitOfWork { get; }
        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        // Task DeleteAsync(T entity);
        Task<T> GetByIdAsync(int id);
        Task<bool> CheckExistence(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        // Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        // IQueryable<T> GetQueryAsync(Expression<Func<T, bool>> predicate);
        // Task<int> CountAsync();
        // Task<IQueryable<T>> GetQueryAsync();
        // Task AttachAsync(T entity);
        // Task DeleteAsync(Expression<Func<T, bool>> criteria);
        // Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> criteria);
        // Task<T> FindOneAsync(Expression<Func<T, bool>> criteria);
        // Task<T> FirstAsync(Expression<Func<T, bool>> predicate);
        // Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        //                               Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        //                               string includeString = null,
        //                               bool disableTracking = true);
        // Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
        //                                Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        //                                List<Expression<Func<T, object>>> includes = null,
        //                                bool disableTracking = true);
        List<string> DefaultPermission();
    }
}