using System.Reflection;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Modeling;
using Redis.OM.Searching;

namespace Common.Redis;

public class CacheRepository<T> : ICacheRepository<T> where T : ICache
{
    public IRedisConnection Connection { get; }
    public IRedisCollection<T> Collection { get; }

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
}