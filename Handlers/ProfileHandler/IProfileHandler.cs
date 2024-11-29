using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.ProfileHandler
{
    public interface IProfileHandler
    {
        Task<int> CreateProfileAsync(string name, 
            string? profilePicture, 
            string streetAndNumber, 
            string postalCode, 
            string city, 
            string email, 
            string phoneNumber);

        Task<List<ProfileDto>> GetProfileNamesAsync();
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
        Task<string> GetProfileNameAsync(int id);
    }
}
