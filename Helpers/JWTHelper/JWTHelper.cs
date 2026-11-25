using Emma.Api.Models;
using Emma.Api.Repositories.ProfileRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Emma.Api.Helpers.JWTHelper
{
    public class JWTHelper(IHttpContextAccessor httpContextAccessor,
        IProfileRepository profileRepository,
        IConfiguration configuration) : IJWTHelper
    {
        private const string Pbkdf2Prefix = "pbkdf2$";
        private const int Pbkdf2Iterations = 100_000;

        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IConfiguration _configuration = configuration;

        private ClaimsPrincipal User => _httpContextAccessor.HttpContext!.User;

        public static string GetSigningKey(IConfiguration configuration)
        {
            var key = configuration["Jwt:Key"];
            return string.IsNullOrWhiteSpace(key) ? "vIgkS94GZw9Pxuoeoefiwlp4vOmj5fQa" : key;
        }

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
                new Claim("profileId", (profileId ?? 0).ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSigningKey(_configuration)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "EmmaLogistics",
                audience: "EmmaLogisticsUser",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>Salted PBKDF2-SHA256 hash, stored as "pbkdf2$iterations$salt$hash".</summary>
        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Pbkdf2Iterations, HashAlgorithmName.SHA256, 32);
            return $"{Pbkdf2Prefix}{Pbkdf2Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>Verifies PBKDF2 hashes and, for older accounts, the legacy unsalted SHA-256 hash.</summary>
        public bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
                return false;

            if (storedHash.StartsWith(Pbkdf2Prefix))
            {
                var parts = storedHash[Pbkdf2Prefix.Length..].Split('$');
                if (parts.Length != 3 || !int.TryParse(parts[0], out var iterations))
                    return false;

                var salt = Convert.FromBase64String(parts[1]);
                var expected = Convert.FromBase64String(parts[2]);
                var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, expected.Length);
                return CryptographicOperations.FixedTimeEquals(actual, expected);
            }

            var legacy = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(legacy), Encoding.UTF8.GetBytes(storedHash));
        }

        public static bool IsLegacyHash(string storedHash) => !storedHash.StartsWith(Pbkdf2Prefix);

        public string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }
    }
}
