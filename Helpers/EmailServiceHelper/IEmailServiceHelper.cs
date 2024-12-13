namespace Emma.Api.Helpers.EmailServiceHelper
{
    public interface IEmailServiceHelper
    {
        void SendEmail(List<string> toEmails, string subject, string body);
    }
}
