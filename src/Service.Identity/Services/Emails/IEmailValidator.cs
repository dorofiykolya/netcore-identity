using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Services.Emails;

public interface IEmailValidator
{
    public Task<bool> ValidateAsync(string address);
}

public class EmailValidator : IEmailValidator
{

    public Task<bool> ValidateAsync(string address)
    {
        return Task.FromResult(MailAddress.TryCreate(address, out var _));
    }
}

public static class EmailValidatorExtensions
{
    public static void AddEmailValidator(this IServiceCollection services)
    {
        services.AddSingleton<IEmailValidator, EmailValidator>();
    }
}