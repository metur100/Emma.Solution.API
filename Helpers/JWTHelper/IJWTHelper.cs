using Emma.Api.Models;

namespace Emma.Api.Helpers.JWTHelper
{
    public interface IJWTHelper
    {
        int GetUserId();
        string GenerateJwtToken(User profile);
        string HashPassword(string password);
    }
}
