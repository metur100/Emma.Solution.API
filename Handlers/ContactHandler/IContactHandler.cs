using Emma.Api.Models;

namespace Emma.Api.Handlers.ContactHandler
{
    public interface IContactHandler
    {
        public List<Contact> GetAllContacts();
    }
}
