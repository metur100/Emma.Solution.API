using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.UserHandler
{
    public interface IUserHandler
    {
        Task<string> RegisterAsync(RegisterUserDto dto);
        Task<string> LoginAsync(string email, string password);
    }
}
