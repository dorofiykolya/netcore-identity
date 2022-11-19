using System;

#pragma warning disable CS8618

namespace Identity.Services.Emails;

[Serializable]
// ReSharper disable once InconsistentNaming
public class EmailOptions
{
    public string From { get; set; }
    public string FromName { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }


    // If you're using Amazon SES in a region other than US West (Oregon), 
    // replace email-smtp.us-west-2.amazonaws.com with the Amazon SES SMTP  
    // endpoint in the appropriate AWS Region.
    public string Host { get; set; } = "email-smtp.us-west-2.amazonaws.com";

    // (Optional) the name of a configuration set to use for this message.
    // If you comment out this line, you also need to remove or comment out
    // the "X-SES-CONFIGURATION-SET" header below.
    public string ConfigSet { get; set; }

    // The port you will connect to on the Amazon SES SMTP endpoint. We
    // are choosing port 587 because we will use STARTTLS to encrypt
    // the connection.
    public int Port { get; set; } = 587;

    // Enable SSL encryption
    public bool EnableSsl { get; set; } = true;
}