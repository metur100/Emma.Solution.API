using Emma.Api.Models;

namespace Emma.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<int> CreateUser(User user);
    }
}
