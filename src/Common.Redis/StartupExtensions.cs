using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Redis.OM;
using Redis.OM.Searching;

namespace Common.Redis;

public static class StartupExtensions
{
    public static void AddRedisCache(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddSingleton(_ => new RedisConnectionProvider(configuration["ConnectionString"]));
        services.AddScoped(typeof(ICacheRepository<>), typeof(CacheRepository<>));
        services.AddScoped(typeof(IRedisCollection<>), typeof(CacheRepository<>));
    }
}