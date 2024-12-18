using Emma.Api.Models;
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

        public async Task<Profile?> GetProfileByIdAsync(int profileId)
        {
            return await _context.Profiles
                .Where(user => user.Id == profileId)
                .FirstOrDefaultAsync();
        }

        public async Task<int?> GetProfileIdByUserIdAsync(int userId)
        {
            var profile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return profile?.Id;
        }

        public async Task<List<int>> GetProfileIdsByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<string>> GetProfileNamesByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => p.Name)
                .ToListAsync();
        }

        public async Task<List<int>> GetUserProfileIdsByUserIdAsync(int userId)
        {
            return await _context.Set<Profile>()
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<Profile>> GetAllProfilesByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task<Profile?> GetProfileInfoByIdAsync(int id)
        {
            return await _context.Profiles
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetProfileIdById(int profileId)
        {
            var user = await _context.Profiles
                .Where(p => p.Id == profileId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            return user;
        }

        public async Task<string?> GetProfileNameByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Profile>> GetProfilesByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .Where(p => p.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateProfileAsync(Profile profile)
        {
            _context.Set<Profile>().Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProfileAsync(int profileId)
        {
            var profile = await GetProfileByIdAsync(profileId);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountProfilesByUserIdAsync(int userId)
        {
            return await _context.Profiles
                .CountAsync(p => p.UserId == userId);
        }

        public async Task<string?> GetProfileNameByProfileIdAsync(int profileId)
        {
            return await _context.Profiles
                .Where(p => p.Id == profileId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetEmailByProfileId(int profileId)
        {
            return await _context.Profiles
                .Where(profile => profile.Id == profileId)
                .Join(_context.Contacts,
                      profile => profile.ContactId,
                      contact => contact.Id,
                      (profile, contact) => contact.Email)
                .FirstOrDefaultAsync();
        }
    }
}
