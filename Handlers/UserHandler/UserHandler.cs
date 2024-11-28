using Emma.Api.Models;
using Emma.Api.Repositories.UserRepository;
using System.Security.Cryptography;
using System.Text;

namespace Emma.Api.Handlers.UserHandler
{
    public class UserHandler(IUserRepository userRepository) : IUserHandler
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<string> RegisterAsync(User profile)
        {
            var existingProfile = await _userRepository.GetProfileByEmailAsync(profile.Email);
            if (existingProfile != null)
            {
                return "Profile already exists.";
            }

            // Hash the password and save the profile
            profile.Password = HashPassword(profile.Password);
            await _userRepository.AddProfileAsync(profile);

            return "Profile registered successfully.";
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
