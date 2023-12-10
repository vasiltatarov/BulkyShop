namespace Bulky.Data.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext dbContext;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.dbSet = dbContext.Set<T>();
    }

    public void Add(T entity)
    {
        this.dbSet.Add(entity);
    }

    public T Get(Expression<Func<T, bool>> filter, string includeProperties = null, bool tracked = false)
    {
        IQueryable<T> query;

        if (tracked)
        {
            query = this.dbSet;
        }
        else
        {
            query = this.dbSet.AsNoTracking();
        }

        query = query.Where(filter);

        if (!string.IsNullOrEmpty(includeProperties))
        {
            var splitIncludeProperties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var includeProp in splitIncludeProperties)
            {
                query = query.Include(includeProp);
            }
        }

        return query.FirstOrDefault();
    }

    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string includeProperties = null)
    {
        IQueryable<T> query = this.dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrEmpty(includeProperties))
        {
            var splitIncludeProperties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var includeProp in splitIncludeProperties)
            {
                query = query.Include(includeProp);
            }
        }

        return query.AsEnumerable();
    }

    public void Remove(T entity)
    {
        this.dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        this.dbSet.RemoveRange(entities);
    }
}
