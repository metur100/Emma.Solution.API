using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.UserRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Emma.Api.Handlers.UserHandler
{
    public class UserHandler(IUserRepository userRepository) : IUserHandler
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            var existingProfile = await _userRepository.GetProfileByEmailAsync(dto.Email);
            if (existingProfile != null)
            {
                return "Profile already exists."; 
            }

            var hashedPassword = HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Password = hashedPassword
            };

            var address = new Address
            {
                StreetAndNumber = dto.StreetAndNumber,
                PostalCode = dto.PostalCode,
                City = dto.City
            };

            var contact = new Contact
            {
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            await _userRepository.RegisterUserAsync(user, address, contact, dto.UserName);

            return "Profile registered successfully."; 
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var hashedPassword = HashPassword(password);

            var user = await _userRepository.GetProfileByEmailAndPasswordAsync(email, hashedPassword);
            if (user == null)
            {
                return "Invalid email or password.";
            }

            return GenerateJwtToken(user); 
        }


        private string GenerateJwtToken(User profile)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, profile.Email),
                new Claim(ClaimTypes.NameIdentifier, profile.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("vIgkS94GZw9Pxuoeoefiwlp4vOmj5fQa"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "EmmaLogistics",  // Use your own issuer
                audience: "EmmaLogisticsUser",  // Use your own audience
                claims: claims,
                expires: DateTime.Now.AddDays(1), // Set token expiration
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token); // Return the JWT token as a string
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
