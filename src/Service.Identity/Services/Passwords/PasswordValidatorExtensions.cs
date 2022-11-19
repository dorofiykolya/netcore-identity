using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Passwords;

public static class PasswordValidatorExtensions
{
    public static void AddPasswordValidator(this IServiceCollection services, IConfigurationSection configuration)
    {
        services.AddSingleton(_ => configuration.Get<PasswordOptions>());
        services.AddSingleton<IPasswordValidator, PasswordValidator>();
    }
}