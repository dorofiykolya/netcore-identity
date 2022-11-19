using Common.Redis;
using Redis.OM.Modeling;

namespace Identity.Repositories.Caches;

[Document(StorageType = StorageType.Json, Prefixes = new[] {nameof(UserEmailBlockCache)})]
public class UserEmailBlockCache : ICache
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    [RedisIdField] [Indexed] public string Email { get; set; } = null!;
    public int InvalidPasswordCount { get; set; }
}