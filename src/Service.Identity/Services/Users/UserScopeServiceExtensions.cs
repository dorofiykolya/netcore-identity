using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Users;

public static class UserScopeServiceExtensions
{
    public static void AddUserScopeProvider(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddSingleton(p =>
        {
            var options = section.Get<UserRoleScopes[]>();
            return options;
        });
        services.AddSingleton<IUserScopeProvider, UserScopeProvider>();
    }
}