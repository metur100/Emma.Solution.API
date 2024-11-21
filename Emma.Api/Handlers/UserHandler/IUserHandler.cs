using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.UserHandler
{
    public interface IUserHandler
    {
        Task<int> CreateUserAsync(string name, 
            string? profilePicture, 
            string streetAndNumber, 
            string postalCode, 
            string city, 
            string email, 
            string phoneNumber);

        Task<List<UserDto>> GetUserNamesAsync();
    }
}
