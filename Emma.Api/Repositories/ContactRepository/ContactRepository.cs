using Emma.Api.Models;

namespace Emma.Api.Repositories.ContactRepository
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;

        public ContactRepository (AppDbContext context)
        {
            _context = context;
        }

        public List<Contact> GetAllContacts()
        {
            return _context.Contacts.ToList();
        }
    }
}
