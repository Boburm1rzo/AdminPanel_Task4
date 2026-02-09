using System.Net;
using System.Net.Mail;

namespace Task4.Services;

public sealed class EmailService(IConfiguration configuration)
{
    public async Task SendConfirmationEmailAsync(string userEmail, string confirmationLink)
    {
        try
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
                Subject = "Confirm your registration",
                Body = $"Please confirm your registration by clicking here: <a href='{confirmationLink}'>Confirm Email</a>",
                IsBodyHtml = true,
            };

            emailMessage.To.Add(userEmail);

            await client.SendMailAsync(emailMessage);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
