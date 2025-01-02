using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.UserHandler
{
    public interface IUserHandler
    {
        Task<string> LoginAsync(string email, string password, HttpResponse response);
        Task<string> RegisterUserWithConfirmationAsync(RegisterUserDto registerUserDto);
        Task<string> ConfirmUserAsync(string token);
        Task<string> ForgotPasswordAsync(string email);
        Task<string> ResetPasswordAsync(string token, string newPassword);
        Task<string?> RefreshTokenAsync(string refreshToken, HttpResponse response);
    }
}
