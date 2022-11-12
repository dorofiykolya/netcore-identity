using System.Threading.Tasks;

namespace Identity.Services.Emails;

public interface IEmailSender
{
    Task SendAsync(string htmlBody, string mailTo, string subject);
}