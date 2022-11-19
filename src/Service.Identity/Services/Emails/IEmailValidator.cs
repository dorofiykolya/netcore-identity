using System.Threading.Tasks;

namespace Identity.Services.Emails;

public interface IEmailValidator
{
    public Task<bool> ValidateAsync(string address);
}