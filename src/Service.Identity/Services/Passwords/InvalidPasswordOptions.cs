using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Passwords;

[Serializable]
public class InvalidPasswordOptions
{
    public int InvalidCountToBlock { get; set; } = 5;
    public TimeSpan BlockTime { get; set; } = TimeSpan.FromMinutes(2);

    public string ToBlockTimeHumanReadable() => $"{BlockTime.Hours:00}:{BlockTime.Minutes:00}:{BlockTime.Seconds:00}";
}

public static class InvalidPasswordExtensions
{
    public static void AddInvalidPasswordOptions(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddSingleton<InvalidPasswordOptions>(_ =>
        {
            var option = new InvalidPasswordOptions();
            section.Bind(option);
            return option;
        });
    }
}