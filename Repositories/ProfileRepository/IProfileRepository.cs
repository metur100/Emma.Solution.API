using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<int> CreateProfileAsync(Profile profile, int userId);
        Task<string?> GetProfileNameByUserIdAsync(int userId);
        Task<List<Profile>> GetProfilesByUserIdAsync(int userId);
        Task UpdateProfileAsync(Profile profile);
        Task DeleteProfileAsync(int profileId);
        Task<Profile?> GetProfileByIdAsync(int profileId);
        Task<List<int>> GetUserProfileIdsByUserIdAsync(int userId);
        Task<int> GetProfileIdById(int profileId);
        Task<List<Profile>> GetAllProfilesByUserIdAsync(int userId);
        Task<Profile?> GetProfileInfoByIdAsync(int id);
        Task<int> CountProfilesByUserIdAsync(int userId);
        Task<int?> GetProfileIdByUserIdAsync(int userId);
        Task<List<string>> GetProfileNamesByUserIdAsync(int userId);
        Task<List<int>> GetProfileIdsByUserIdAsync(int userId);
        Task<string?> GetProfileNameByProfileIdAsync(int profileId);
    }
}
