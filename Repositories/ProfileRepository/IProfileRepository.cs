using Emma.Api.Models;

namespace Emma.Api.Repositories.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<User?> GetProfileByEmailAsync(string email);
        Task AddProfileAsync(User profile);
        Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword);
    }
}
