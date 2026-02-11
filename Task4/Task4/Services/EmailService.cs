using System.Net;
using System.Net.Mail;
using Task4.Helpers;

namespace Task4.Services;

public sealed class EmailService(IConfiguration configuration)
{
    public async Task SendConfirmationEmailAsync(string userEmail, string confirmationLink)
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

        var body = EmailTemplateHelper.BuildConfirmationBody(confirmationLink);

        var emailMessage = new MailMessage
        {
            From = new MailAddress(senderEmail!),
            Subject = "Confirm your email",
            Body = body,
            IsBodyHtml = true,
        };

        emailMessage.To.Add(userEmail);

        await client.SendMailAsync(emailMessage);
    }
}
