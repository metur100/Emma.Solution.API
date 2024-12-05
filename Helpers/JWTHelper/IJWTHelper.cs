using Emma.Api.Models;

namespace Emma.Api.Helpers.JWTHelper
{
    public interface IJWTHelper
    {
        int GetUserId();
        Task<string> GenerateJwtTokenAsync(User user);
        string HashPassword(string password);
    }
}
