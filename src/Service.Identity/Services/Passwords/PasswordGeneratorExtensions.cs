using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Passwords;

public static class PasswordGeneratorExtensions
{
    public static void AddPasswordGenerator(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddSingleton<PasswordOptions>(_ => configuration.Get<PasswordOptions>());
        services.AddSingleton<IPasswordGenerator, PasswordGenerator>();
    }
}