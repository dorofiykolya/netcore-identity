using Redis.OM.Searching;

namespace Common.Redis;

public interface ICacheRepository
{

}

public interface ICacheRepository<T> : ICacheRepository, IRedisCollection<T> where T : ICache
{
    IRedisCollection<T> Collection { get; }
}