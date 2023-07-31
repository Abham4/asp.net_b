namespace backend_r.Infrastructure.Utils
{
    public class BaseRepository<T> : IBaseRepository<T> where T : EntityBase
    {
        private readonly DbContext _context;
        private IUnitOfWork unitOfWork;
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if(unitOfWork == null)
                {
                    unitOfWork = new UnitOfWork(this._context);
                }
                return unitOfWork;
            }
            set
            {
                unitOfWork = new UnitOfWork(this._context);
            }
        }

        public BaseRepository(JoshuaContext con)
        {
            _context = con ?? throw new ArgumentNullException("context");
        }

        public virtual async Task AddAsync(T entity)
        {
            if(entity == null)
                throw new ArgumentNullException("entity");
            await _context.Set<T>().AddAsync(entity);
        }

        // public virtual async Task DeleteAsync(T entity)
        // {
        //     if(entity == null)
        //         throw new ArgumentNullException("entity");
        //     _context.Set<T>().Remove(entity);
        // }

        public virtual async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
        // {
        //     return await _context.Set<T>().Where(predicate).ToListAsync();
        // }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // public IQueryable<T> GetQueryAsync(Expression<Func<T, bool>> predicate)
        // {
        //     return _context.Set<T>().Where(predicate);
        // }

        public virtual void UpdateAsync(T entity)
        {
            if(entity == null)
                throw new ArgumentNullException("entity");
                
            _context.Entry(entity).State = EntityState.Modified;
            _context.Set<T>().Update(entity);
        }

        // public async Task<int> CountAsync()
        // {
        //     return await (await GetQueryAsync()).CountAsync();
        // }

        // public async Task<IQueryable<T>> GetQueryAsync()
        // {
        //     IQueryable<T> query = _context.Set<T>();
        //     return query;
        // }

        // public async Task AttachAsync(T entity)
        // {
        //     if (entity == null)
        //         throw new ArgumentNullException("entity");
        //     _context.Set<T>().Attach(entity);
        // }

        // public async Task DeleteAsync(Expression<Func<T, bool>> criteria)
        // {
        //     IEnumerable<T> records = await FindAsync(criteria);

        //     foreach (T record in records)
        //     {
        //         await DeleteAsync(record);
        //     }
        // }

        // public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> criteria)
        // {
        //     return (await GetQueryAsync()).Where(criteria);
        // }

        // public async Task<T> FindOneAsync(Expression<Func<T, bool>> criteria)
        // {
        //     return await (await GetQueryAsync()).Where(criteria).FirstOrDefaultAsync();
        // }

        // public async Task<T> FirstAsync(Expression<Func<T, bool>> predicate)
        // {
        //     return await(await GetQueryAsync()).FirstAsync(predicate);
        // }

        // public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        // {
        //     IQueryable<T> query = _context.Set<T>();
        //     if (disableTracking) query = query.AsNoTracking();

        //     if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

        //     if (predicate != null) query = query.Where(predicate);

        //     if (orderBy != null)
        //         return await orderBy(query).ToListAsync();
                
        //     return await query.ToListAsync();
        // }

        // public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        // {
        //     IQueryable<T> query = _context.Set<T>();
        //     if (disableTracking) query = query.AsNoTracking();

        //     if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

        //     if (predicate != null) query = query.Where(predicate);

        //     if (orderBy != null)
        //         return await orderBy(query).ToListAsync();

        //     return await query.ToListAsync();
        // }

        public async Task<bool> CheckExistence(int id)
        {
            return await _context.Set<T>().AnyAsync(c => c.Id == id);
        }

        public virtual List<string> DefaultPermission()
        {
            var entity = typeof(T).ToString().TrimStart("backend_r.Domain.Entities.".ToCharArray());

            return new List<string>
            {
                $"AuthorizedTo.{entity}.Add",
                $"AuthorizedTo.{entity}.View",
                $"AuthorizedTo.{entity}.Edit",
                $"AuthorizedTo.{entity}.Remove"
            };
        }
    }
}