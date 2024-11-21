using Emma.Api.Models;

namespace Emma.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(User user);
        Task<List<string>> GetUserNamesAsync();
    }
}
