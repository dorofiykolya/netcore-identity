using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Services.Emails;

public class Sender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<Sender> _logger;
    public Sender(EmailOptions options, ILogger<Sender> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task SendAsync(string htmlBody, string mailTo, string subject)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(_options.From, _options.FromName);
        mailMessage.To.Add(new MailAddress(mailTo));
        mailMessage.Subject = subject;
        mailMessage.Body = htmlBody;

        if (!string.IsNullOrWhiteSpace(_options.ConfigSet))
        {
            mailMessage.Headers.Add("X-SES-CONFIGURATION-SET", _options.ConfigSet);
        }

        using (var client = new SmtpClient(_options.Host, _options.Port))
        {
            // Pass SMTP credentials
            client.Credentials = new NetworkCredential(_options.SmtpUsername, _options.SmtpPassword);

            // Enable SSL encryption
            client.EnableSsl = _options.EnableSsl;

            try
            {
                _logger.LogInformation("Attempting to send email {To}", mailTo);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent! {To}", mailTo);
            }
            catch (Exception ex)
            {
                _logger.LogError("The email was not sent. Error {Message}", ex.Message);
                throw;
            }
        }
    }
}

public static class SenderExtensions
{
    public static void AddEmailSender(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddSingleton(_ => section.Get<EmailOptions>());
        services.AddScoped<IEmailSender, Sender>();
    }
}