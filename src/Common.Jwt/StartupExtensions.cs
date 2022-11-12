using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Jwt;

public static class StartupExtensions
{
    public static void AddJwtGenerator(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddSingleton<JwtOptions>(_ => configuration.Get<JwtOptions>());
        services.AddSingleton<IJwtGenerator, JwtGenerator>();
    }
}
