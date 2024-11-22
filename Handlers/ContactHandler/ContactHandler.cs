using Emma.Api.Models;
using Emma.Api.Repositories.ContactRepository;

namespace Emma.Api.Handlers.ContactHandler
{
    public class ContactHandler(IContactRepository contactRepository) : IContactHandler
    {
        private readonly IContactRepository _contactRepository = contactRepository;

        public List<Contact> GetAllContacts()
        {
            return _contactRepository.GetAllContacts();
        }
    }
}
