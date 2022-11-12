using Common.Redis;
using Redis.OM.Modeling;

namespace Identity.Repositories.Caches;

[Document(StorageType = StorageType.Json, Prefixes = new[] {nameof(UserTokenCache)})]
public class UserTokenCache : ICache
{
    [RedisIdField] [Indexed] public string UserId { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}