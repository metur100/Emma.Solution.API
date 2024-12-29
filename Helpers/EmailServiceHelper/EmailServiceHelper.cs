using DinkToPdf;
using MailKit.Security;
using MimeKit;

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


        public void SendEmailWithAttachment(
            List<string> toEmails,
            string subject,
            string body,
            byte[] attachment,
            string attachmentFileName)
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

            // Create the email body and attachment
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };

            if (attachment != null && attachment.Length > 0)
            {
                bodyBuilder.Attachments.Add(attachmentFileName, attachment, ContentType.Parse("application/pdf"));
            }

            message.Body = bodyBuilder.ToMessageBody();

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

        public byte[] GeneratePdfFromHtml(string htmlContent)
        {
            var converter = new SynchronizedConverter(new PdfTools());
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                PaperSize = PaperKind.A4,
            },
                Objects = {
                    new ObjectSettings() {
                    HtmlContent = htmlContent,
                    WebSettings = {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "customStyles.css") // Reference your CSS file
                     }
                 }
             }
            };

            return converter.Convert(doc);
        }
    }
}
