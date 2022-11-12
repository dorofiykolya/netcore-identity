using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Identity.Services.Emails;

public class EmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailSender> _logger;
    public EmailSender(EmailOptions options, ILogger<EmailSender> logger)
    {
        _options = options;
        _logger = logger;
    }
    
    public async Task SendAsync(string body, string to, string subject)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(_options.From, _options.FromName);
        mailMessage.To.Add(new MailAddress(to));
        mailMessage.Subject = subject;
        mailMessage.Body = body;

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
                _logger.LogInformation("Attempting to send email {To}", to);
                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent! {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError("The email was not sent. Error {Message}", ex.Message);
                throw;
            }
        }
    }
}

public static class EmailSenderExtensions
{
    public static void AddEmailSender(this IServiceCollection services, IConfigurationSection section)
    {
        services.AddSingleton<EmailOptions>(_ => section.Get<EmailOptions>());
        services.AddScoped<IEmailSender, EmailSender>();
    }
}