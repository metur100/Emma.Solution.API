using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;

namespace Emma.Api.Helpers.EmailServiceHelper
{
    public class EmailServiceHelper(IConfiguration configuration) : IEmailServiceHelper
    {
        private readonly IConfiguration _config = configuration;

        public void SendEmail(List<string> toEmails, string subject, string body)
        {
            var port = _config.GetValue<int>("SMTP:Port");
            var host = _config.GetValue<string>("SMTP:Host");
            var password = _config.GetValue<string>("SMTP:EmailPassword");
            var fromEmail = _config.GetValue<string>("SMTP:EmailAddress");

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Emma Logistics", fromEmail));

            foreach (var toEmail in toEmails)
            {
                message.To.Add(new MailboxAddress("Emma Logistics", toEmail));
            }

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(host, port, SecureSocketOptions.StartTls);
                    client.Authenticate(fromEmail, password);
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }
    }
}
