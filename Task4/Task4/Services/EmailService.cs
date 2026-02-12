using System.Net;
using System.Net.Mail;
using Task4.Helpers;

namespace Task4.Services;

public sealed class EmailService(IConfiguration configuration)
{
    public async Task SendConfirmationEmailAsync(string userEmail, string confirmationLink)
    {
        var body = EmailTemplateHelper.BuildConfirmationBody(confirmationLink);

        await SendAsync(userEmail, "Confirm your email", body);
    }

    public async Task SendResetPasswordLinkAsync(string email, string link)
    {
        var body = EmailTemplateHelper.BuildResetPasswordBody(link);

        await SendAsync(email, "Reset your password", body);
    }

    private async Task SendAsync(string email, string subject, string body)
    {
        var smtpServer = configuration["Email:Host"];
        var port = int.Parse(configuration["Email:Port"]!);
        var senderEmail = configuration["Email:From"];
        var password = configuration["Email:Password"];

        using var client = new SmtpClient(smtpServer, port)
        {
            Credentials = new NetworkCredential(senderEmail, password),
            EnableSsl = true,
        };

        var emailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail!),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        emailMessage.To.Add(email);

        await client.SendMailAsync(emailMessage);
    }
}
