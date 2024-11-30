using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<int> CreateProfileAsync(Profile profile, int userId);
        Task<List<ProfileDto>> GetProfilesByUserIdAsync(int userId);
        Task<int> GetProfileById(int id);
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
        Task<string> GetProfileNameByUserId(int userId);
    }
}
