using System.Net.Mail;
using System.Threading.Tasks;

namespace Identity.Services.Emails;

// ReSharper disable once InconsistentNaming
public class EmailValidator : IEmailValidator
{
    public Task<bool> ValidateAsync(string address)
    {
        return Task.FromResult(MailAddress.TryCreate(address, out _));
    }
}