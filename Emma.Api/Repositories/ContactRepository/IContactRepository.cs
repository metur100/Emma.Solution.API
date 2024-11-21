using Emma.Api.Models;

namespace Emma.Api.Repositories.ContactRepository
{
    public interface IContactRepository
    {
        public List<Contact> GetAllContacts();
    }
}
