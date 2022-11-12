using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Mongo;

public static class StartupExtensions
{
    public static void AddMongoRepository(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddSingleton<MongoOptions>(_ => configuration.Get<MongoOptions>());
        services.AddScoped<IMongoClient, MongoClientImpl>();
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
    }
}
