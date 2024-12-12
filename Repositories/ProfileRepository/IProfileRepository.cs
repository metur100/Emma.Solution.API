using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<int> CreateProfileAsync(Profile profile, int userId);
        Task<List<ProfileDto>> GetProfileNamesByUserIdAsync(int userId);
        Task<int> GetProfileIdById(int id);
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
        Task<string> GetProfileNameByUserId(int userId);
        Task<IEnumerable<ProfileManagementDTO>> GetProfilesByUserIdAsync(int userId);
        Task UpdateProfileAsync(int profileId, UpdateProfileDTO updateProfileDTO);
        Task<UpdateProfileDTO> GetUpdateProfileByIdAsync(int id);
        Task DeleteProfileByIdAsync(int id);
        Task<Profile?> GetProfileByIdAsync(int profileId);
        Task<List<int>> GetUserProfileIdsByUserIdAsync(int userId);
        Task<int> GetProfileIdByIdAsync(int profileId);
    }
}
