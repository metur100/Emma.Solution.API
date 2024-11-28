using Emma.Api.Models;
using Emma.Api.Repositories.ProfileRepository;
using System.Security.Cryptography;
using System.Text;

namespace Emma.Api.Handlers.ProfileHandler
{
    public class ProfileHandler(ProfileRepository profileRepository) : IProfileHandler
    {
        private readonly ProfileRepository _profileRepository = profileRepository;

        public async Task<string> RegisterAsync(User profile)
        {
            // Check if profile already exists
            var existingProfile = await _profileRepository.GetProfileByEmailAsync(profile.Email);
            if (existingProfile != null)
            {
                return "Profile already exists.";
            }

            // Hash the password and save the profile
            profile.Password = HashPassword(profile.Password);
            await _profileRepository.AddProfileAsync(profile);

            return "Profile registered successfully.";
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            // Hash the password for comparison
            var hashedPassword = HashPassword(password);

            // Verify profile credentials
            var profile = await _profileRepository.GetProfileByEmailAndPasswordAsync(email, hashedPassword);
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
