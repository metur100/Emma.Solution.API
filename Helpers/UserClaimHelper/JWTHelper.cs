using Emma.Api.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Emma.Api.Helpers.UserClaimHelper
{
    public class JWTHelper(ClaimsPrincipal user) : IJWTHelper
    {
        private readonly ClaimsPrincipal _user = user;

        public int GetUserId()
        {
            var userIdClaim = _user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User ID claim is missing or invalid.");

            return int.Parse(userIdClaim);
        }

        public string GenerateJwtToken(User profile)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, profile.Email),
                new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vIgkS94GZw9Pxuoeoefiwlp4vOmj5fQa"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "EmmaLogistics",
                audience: "EmmaLogisticsUser",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
