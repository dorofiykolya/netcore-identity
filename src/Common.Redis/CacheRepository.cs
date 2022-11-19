using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Modeling;
using Redis.OM.Searching;

namespace Common.Redis;

public class CacheRepository<T> : ICacheRepository<T> where T : ICache
{

    public CacheRepository(RedisConnectionProvider provider)
    {
        if (typeof(T).GetCustomAttribute<DocumentAttribute>() == null)
        {
            throw new ArgumentException($"type: {typeof(T)} must be had a {typeof(DocumentAttribute)} attribute");
        }

        Connection = provider.Connection;
        Collection = provider.RedisCollection<T>();

        Connection.CreateIndex(typeof(T));
    }
    public IRedisConnection Connection { get; }
    public IRedisCollection<T> Collection { get; }
    public IEnumerator<T> GetEnumerator()
    {
        return Collection.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Collection).GetEnumerator();
    }
    public Type ElementType
    {
        get{
            return Collection.ElementType;
        }
    }
    public Expression Expression
    {
        get{
            return Collection.Expression;
        }
    }
    public IQueryProvider Provider
    {
        get{
            return Collection.Provider;
        }
    }
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
    {
        return Collection.GetAsyncEnumerator(cancellationToken);
    }
    public void Save()
    {
        Collection.Save();
    }
    public ValueTask SaveAsync()
    {
        return Collection.SaveAsync();
    }
    public string Insert(T item)
    {
        return Collection.Insert(item);
    }
    public string Insert(T item, TimeSpan timeSpan)
    {
        return Collection.Insert(item, timeSpan);
    }
    public Task<string> InsertAsync(T item)
    {
        return Collection.InsertAsync(item);
    }
    public Task<string> InsertAsync(T item, TimeSpan timeSpan)
    {
        return Collection.InsertAsync(item, timeSpan);
    }
    public Task<T?> FindByIdAsync(string id)
    {
        return Collection.FindByIdAsync(id);
    }
    public T? FindById(string id)
    {
        return Collection.FindById(id);
    }
    public bool Any(Expression<Func<T, bool>> expression)
    {
        return Collection.Any(expression);
    }
    public void Update(T item)
    {
        Collection.Update(item);
    }
    public Task UpdateAsync(T item)
    {
        return Collection.UpdateAsync(item);
    }
    public void Delete(T item)
    {
        Collection.Delete(item);
    }
    public Task DeleteAsync(T item)
    {
        return Collection.DeleteAsync(item);
    }
    public Task<IList<T>> ToListAsync()
    {
        return Collection.ToListAsync();
    }
    public Task<int> CountAsync()
    {
        return Collection.CountAsync();
    }
    public Task<int> CountAsync(Expression<Func<T, bool>> expression)
    {
        return Collection.CountAsync(expression);
    }
    public Task<bool> AnyAsync()
    {
        return Collection.AnyAsync();
    }
    public Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        return Collection.AnyAsync(expression);
    }
    public Task<T> FirstAsync()
    {
        return Collection.FirstAsync();
    }
    public Task<T> FirstAsync(Expression<Func<T, bool>> expression)
    {
        return Collection.FirstAsync(expression);
    }
    public Task<T?> FirstOrDefaultAsync()
    {
        return Collection.FirstOrDefaultAsync();
    }
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        return Collection.FirstOrDefaultAsync(expression);
    }
    public Task<T> SingleAsync()
    {
        return Collection.SingleAsync();
    }
    public Task<T> SingleAsync(Expression<Func<T, bool>> expression)
    {
        return Collection.SingleAsync(expression);
    }
    public Task<T?> SingleOrDefaultAsync()
    {
        return Collection.SingleOrDefaultAsync();
    }
    public Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        return Collection.SingleOrDefaultAsync(expression);
    }
    public int Count(Expression<Func<T, bool>> expression)
    {
        return Collection.Count(expression);
    }
    public T First(Expression<Func<T, bool>> expression)
    {
        return Collection.First(expression);
    }
    public T? FirstOrDefault(Expression<Func<T, bool>> expression)
    {
        return Collection.FirstOrDefault(expression);
    }
    public T Single(Expression<Func<T, bool>> expression)
    {
        return Collection.Single(expression);
    }
    public T? SingleOrDefault(Expression<Func<T, bool>> expression)
    {
        return Collection.SingleOrDefault(expression);
    }
    public Task<IDictionary<string, T?>> FindByIdsAsync(IEnumerable<string> ids)
    {
        return Collection.FindByIdsAsync(ids);
    }
    public bool SaveState
    {
        get{
            return Collection.SaveState;
        }
    }
    public RedisCollectionStateManager StateManager
    {
        get{
            return Collection.StateManager;
        }
    }
    public int ChunkSize
    {
        get{
            return Collection.ChunkSize;
        }
    }
}