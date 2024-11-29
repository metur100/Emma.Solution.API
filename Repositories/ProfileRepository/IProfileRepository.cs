using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.ProfileRepository
{
    public interface IProfileRepository
    {
        Task<int> CreateProfileAsync(Profile user);
        Task<List<ProfileDto>> GetProfileNamesAsync();
        Task<int> GetProfileById(int id);
        Task<string> GetProfileNameById(int id);
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
    }
}
