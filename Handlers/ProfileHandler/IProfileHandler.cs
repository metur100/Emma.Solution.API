using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.ProfileHandler
{
    public interface IProfileHandler
    {
        Task<int> CreateUserAsync(string name, 
            string? profilePicture, 
            string streetAndNumber, 
            string postalCode, 
            string city, 
            string email, 
            string phoneNumber);

        Task<List<ProfileDto>> GetUserNamesAsync();
        Task<ProfileInfoDTO> GetUserProfileInformations(int id);
    }
}
