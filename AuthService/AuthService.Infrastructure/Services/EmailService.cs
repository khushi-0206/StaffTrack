using AuthService.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AuthService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var from = _config["Email:From"];
        var user = _config["Email:User"];
        var pass = _config["Email:Pass"];
        var smtpHost = _config["Email:Smtp"];
        var port = _config.GetValue("Email:Port", 587);

        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(smtpHost))
            throw new InvalidOperationException("Email settings are not configured.");

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        //message.Body = new TextPart("plain") { Text = body };
        message.Body = new TextPart("html") { Text = body };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
            throw new Exception("Email credentials missing");

        await smtp.AuthenticateAsync(user, pass, cancellationToken);

        await smtp.SendAsync(message, cancellationToken);

        await smtp.DisconnectAsync(true, cancellationToken);
    }
}
