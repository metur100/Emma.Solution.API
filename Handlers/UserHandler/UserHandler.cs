using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.UserRepository;
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
                return "Profile already exists."; // Return a message string
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

            await _userRepository.RegisterUserAsync(user, address, contact);

            return "Profile registered successfully."; // Return success message
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var hashedPassword = HashPassword(password);

            var profile = await _userRepository.GetProfileByEmailAndPasswordAsync(email, hashedPassword);
            if (profile == null)
            {
                return "Invalid email or password.";
            }

            return "Login successful.";
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
