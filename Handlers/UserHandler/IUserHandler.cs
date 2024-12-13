using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.UserHandler
{
    public interface IUserHandler
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> RegisterUserAsync(RegisterUserDto registerUserDto);
        Task<string> RegisterUserWithConfirmationAsync(RegisterUserDto registerUserDto);
        Task<string> ConfirmUserAsync(string token);
    }
}
