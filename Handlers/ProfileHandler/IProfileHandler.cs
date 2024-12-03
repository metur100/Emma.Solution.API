using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.ProfileHandler
{
    public interface IProfileHandler
    {
        Task<int> CreateProfileAsync(
            string name,
            string? profilePicture,
            string streetAndNumber,
            string postalCode,
            string city,
            string email,
            string phoneNumber,
            int userId);

        Task<List<ProfileDto>> GetProfileNamesByUserIdAsync(int userId);
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
        Task<string> GetProfileNameByUserId(int userId);
        Task<IEnumerable<ProfileManagementDTO>> GetProfilesByUserIdAsync(int userId);
        Task UpdateProfileAsync(int profileId, UpdateProfileDTO updateProfileDTO);
        Task<UpdateProfileDTO> HandleGetProfileByIdAsync(int id);
        Task HandleDeleteProfileByIdAsync(int id);
    }
}
