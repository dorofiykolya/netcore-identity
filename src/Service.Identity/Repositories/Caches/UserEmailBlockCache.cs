using System.Threading.Tasks;
using Common.Redis;
using Identity.Services.Identities;
using Identity.Services.Passwords;
using Redis.OM.Modeling;

namespace Identity.Repositories.Caches;

[Document(StorageType = StorageType.Json, Prefixes = new[] {nameof(UserEmailBlockCache)})]
public class UserEmailBlockCache : ICache
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    [RedisIdField] [Indexed] public string Email { get; set; } = null!;
    public int InvalidCount { get; set; }
}

public static class UserEmailBlockCacheExtensions
{
    public static async Task IncreaseInvalidCount(this ICacheRepository<UserEmailBlockCache> repository, string email, UserEmailBlockCache? emailCache, InvalidPasswordOptions invalidPasswordOptions)
    {
        if (emailCache != null)
        {
            emailCache.InvalidCount++;
            await repository.UpdateAsync(emailCache);
            await repository.SaveAsync();
        }
        else
        {
            await repository.InsertAsync(new UserEmailBlockCache
            {
                Email = email, InvalidCount = 1
            }, invalidPasswordOptions.BlockTime);
        }
    }

    public static async Task<UserEmailBlockCache?> CheckEmail(this ICacheRepository<UserEmailBlockCache> repository, string email, InvalidPasswordOptions options)
    {
        var emailCache = await repository.FindByIdAsync(email);
        if (emailCache != null)
        {
            if (emailCache.InvalidCount > options.InvalidCountToBlock)
            {
                throw IdentityErrorCode.EmailBlockedDuringTime.Exception(options.ToBlockTimeHumanReadable());
            }
            return emailCache;
        }
        return null;
    }
}