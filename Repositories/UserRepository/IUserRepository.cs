using Emma.Api.Models;

namespace Emma.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetProfileByEmailAsync(string email);
        Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword);
        Task RegisterUserAsync(User user, Address address, Contact contact, string userName);
        Task<int?> GetProfileIdByUserIdAsync(int userId);
        Task AddAsync(User user);
    }
}
