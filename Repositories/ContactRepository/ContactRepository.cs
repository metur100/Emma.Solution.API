using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Contact?> GetContactByIdAsync(int contactId)
        {
            return await _context.Contacts
                .Where(contact => contact.Id == contactId)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateContactAsync(Contact contact)
        {
            _context.Set<Contact>().Update(contact);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContactAsync(int contactId)
        {
            var contact = await GetContactByIdAsync(contactId);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
                await _context.SaveChangesAsync();
            }
        }
    }
}
