using Emma.Api.Models;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.UserRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Emma.Api.Helpers.JWTHelper
{
    public class JWTHelper(IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository,
        IProfileRepository profileRepository) : IJWTHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext!.User;

        public int GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new Exception("User ID claim is missing or invalid.");

            return int.Parse(userIdClaim);
        }

        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            var profileId = await _profileRepository.GetProfileIdByUserIdAsync(user.Id);

            var claims = new[]
            {
                new Claim("email", user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim("profileId", profileId.Value.ToString())
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