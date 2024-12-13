using Emma.Api.Models;

namespace Emma.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<User?> GetProfileByEmailAsync(string email);
        Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword);
        Task AddAsync(User user);
    }
}
