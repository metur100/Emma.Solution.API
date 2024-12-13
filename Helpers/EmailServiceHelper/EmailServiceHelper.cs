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
            // Configuration settings
            var port = _config.GetValue<int>("SMTP:Port");
            var host = _config.GetValue<string>("SMTP:Host");
            var password = _config.GetValue<string>("SMTP:EmailPassword");
            var fromEmail = _config.GetValue<string>("SMTP:EmailAddress");

            // Create a new email message
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("HBS", fromEmail));

            foreach (var toEmail in toEmails)
            {
                message.To.Add(new MailboxAddress("HBS", toEmail));
            }

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Connect to the SMTP server
                    client.Connect(host, port, SecureSocketOptions.StartTls);

                    // Authenticate to the SMTP server
                    client.Authenticate(fromEmail, password);

                    // Send the email
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
