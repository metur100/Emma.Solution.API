using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetProfileByEmailAsync(string email);
        Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword);
        Task RegisterUserAsync(User user, Address address, Contact contact);
    }
}
