namespace Emma.Api.Helpers.EmailServiceHelper
{
    public interface IEmailServiceHelper
    {
        void SendEmail(List<string> toEmails, string subject, string body);
        public void SendEmailWithAttachment(
            List<string> toEmails,
            string subject,
            string body,
            byte[] attachment,
            string attachmentFileName);
        Task<byte[]> GeneratePdfFromHtml(string htmlContent);
    }
}
