using Redis.OM.Searching;

namespace Common.Redis;

public interface ICacheRepository
{

}

public interface ICacheRepository<T> : ICacheRepository where T : ICache
{
    IRedisCollection<T> Collection { get; }
}
