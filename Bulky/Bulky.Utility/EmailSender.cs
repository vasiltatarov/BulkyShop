namespace Bulky.Utility;

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public EmailSender(IConfiguration configuration)
    {
        this.SendGridSecretKey = configuration.GetValue<string>("SendGrid:SecretKey");
    }

    public string SendGridSecretKey { get; set; }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;

        var client = new SendGridClient(this.SendGridSecretKey);
        var from = new EmailAddress("TestUser@gmail.com", "My Website");
        var to = new EmailAddress(email);
        var message = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

        return client.SendEmailAsync(message);
    }
}
