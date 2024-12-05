using Emma.Api.Models;

namespace Emma.Api.Repositories.ContactRepository
{
    public class ContactRepository(AppDbContext context) : IContactRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<int> CreateContact(Contact contact)
        {
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
            return contact.Id;
        }
    }
}
