using Emma.Api.Handlers.UserHandler;
using Emma.Api.Models;
using Emma.Api.Repositories.UserRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Emma.Api.Handlers.BasicAuthenticationHandler
{
    public class BasicAuthenticationHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserHandler _userHandler;

        public BasicAuthenticationHandler (IUserRepository userRepository, IUserHandler userHandler)
        {
            _userRepository = userRepository;
            _userHandler = userHandler;
        }

        public async Task<string> AuthenticateAsync(string email, string password)
        {
            var user = await _userRepository.GetProfileByEmailAsync(email);
            if (user == null || !VerifyPassword(password, user.Password))
            {
                return "Invalid credentials."; 
            }

            return GenerateJwtToken(user);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return _userHandler.HashPassword(password) == hashedPassword;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
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
    }
}
