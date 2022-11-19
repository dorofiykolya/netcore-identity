using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Emails;

// ReSharper disable once InconsistentNaming
public static class EmailValidatorExtensions
{
    public static void AddEmailValidator(this IServiceCollection services)
    {
        services.AddSingleton<IEmailValidator, EmailValidator>();
    }
}