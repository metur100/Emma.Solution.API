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
            var result = _context.Contacts.ToList();
            if(result == null)
            {
                result = new List<Contact>();
            }
            return result;
        }

        public async Task<int> CreateContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact.Id;
        }
    }
}
