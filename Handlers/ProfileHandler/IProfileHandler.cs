using Emma.Api.Models;

namespace Emma.Api.Handlers.ProfileHandler
{
    public interface IProfileHandler
    {
        Task<string> RegisterAsync(User profile);
        Task<string> LoginAsync(string email, string password);
        string HashPassword(string password);
    }
}
