using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<int> CreateUserAsync(Profile user);
        Task<List<ProfileDto>> GetUserNamesAsync();
        Task<int> GetUserById(int id);
        Task<string> GetUserNameById(string userName);
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
    }
}
