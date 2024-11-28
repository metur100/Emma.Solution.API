using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<int> CreateUserAsync(Profile user);
        Task<List<UserDto>> GetUserNamesAsync();
        Task<int> GetUserById(int id);
        Task<string> GetUserNameById(string userName);
        Task<UserProfileDTO> GetUserProfileInformations(int id);
    }
}
