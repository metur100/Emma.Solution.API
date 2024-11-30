using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.ProfileRepository
{
    public class ProfileRepository(AppDbContext context) : IProfileRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<int> CreateProfileAsync(Profile profile, int userId)
        {
            profile.UserId = userId;
            _context.Profiles.Add(profile);

            await _context.SaveChangesAsync();
            return profile.Id;
        }

        public async Task<List<ProfileDto>> GetProfilesByUserIdAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid User ID.");

            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => new ProfileDto
                {
                    Id = p.Id,
                    UserName = p.Name
                })
                .ToListAsync();
        }

        public async Task<int> GetProfileById(int id)
        {
            var profile = await _context.Profiles
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            return profile == null ? throw new Exception("User not found") : profile.Id;
        }

        public async Task<ProfileInfoDTO> GetUserProfileInformations(int id)
        {
            var profile = await _context.Profiles
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    Profile = u,
                    Address = _context.Addresses.FirstOrDefault(a => a.Id == u.AddressId),
                    Contact = _context.Contacts.FirstOrDefault(c => c.Id == u.ContactId)
                })
                .FirstOrDefaultAsync() ?? throw new Exception("User not found");

            var userProfile = new ProfileInfoDTO
            {
                Id = profile.Profile.Id,
                UserName = profile.Profile.Name,
                ProfilePicture = profile.Profile.ProfilePicture,
                StreetAndNumber = profile.Address?.StreetAndNumber ?? string.Empty, 
                PostalCode = profile.Address?.PostalCode ?? string.Empty,
                City = profile.Address?.City ?? string.Empty,
                Email = profile.Contact?.Email ?? string.Empty,
                PhoneNumber = profile.Contact?.PhoneNumber ?? string.Empty,
                Ratings = profile.Profile.Ratings
            };

            return userProfile;
        }

        public async Task<string> GetProfileNameByUserId(int userId)
        {
            var profile = await _context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => p.Name) 
                .FirstOrDefaultAsync();

            if (profile == null)
                throw new Exception($"Profile not found for UserId: {userId}");

            return profile;
        }
    }
}
