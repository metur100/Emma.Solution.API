using Emma.Api.Models;

namespace Emma.Api.Handlers.UserHandler
{
    public interface IUserHandler
    {
        Task<string> RegisterAsync(User profile);
        Task<string> LoginAsync(string email, string password);
        string HashPassword(string password);
    }
}
