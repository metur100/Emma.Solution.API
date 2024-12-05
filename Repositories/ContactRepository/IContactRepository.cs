using Emma.Api.Models;

namespace Emma.Api.Repositories.ContactRepository
{
    public interface IContactRepository
    {
        Task<int> CreateContact(Contact contact);
    }
}
