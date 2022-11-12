using System.Threading.Tasks;

namespace Identity.Services.Emails;

public interface IEmailSender
{
    Task SendAsync(string body, string to, string subject);
}