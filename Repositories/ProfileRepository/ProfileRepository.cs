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

        public async Task<Profile?> GetProfileByIdAsync(int profileId)
        {
            return await _context.Profiles
                .Where(user => user.Id == profileId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetUserProfileIdsByUserIdAsync(int userId)
        {
            return await _context.Set<Profile>()
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<ProfileDto>> GetProfileNamesByUserIdAsync(int userId)
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

        public async Task<int> GetProfileIdById(int id)
        {
            var profile = await _context.Profiles
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            return profile == null ? throw new Exception("User not found") : profile.Id;
        }

        public async Task<int> GetProfileIdByIdAsync(int profileId)
        {
            var user = await _context.Profiles
                .Where(p => p.Id == profileId)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();

            return user;
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

        public async Task<IEnumerable<ProfileManagementDTO>> GetProfilesByUserIdAsync(int userId)
        {
            var userProfileIds = await _context.Set<Profile>()
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            if (!userProfileIds.Any())
            {
                return Enumerable.Empty<ProfileManagementDTO>();
            }

            return await _context.Set<Profile>()
                .Where(p => userProfileIds.Contains(p.Id))
                .Join(_context.Set<Address>(),
                      p => p.AddressId, 
                      a => a.Id, 
                      (profile, address) => new ProfileManagementDTO
                      {
                          Id = profile.Id,
                          Name = profile.Name,
                          City = address.City 
                      })
                .ToListAsync();
        }

        public async Task UpdateProfileAsync(int profileId, UpdateProfileDTO updateProfileDTO)
        {
            // Retrieve the profile by profileId
            var profile = await _context.Set<Profile>()
                .FirstOrDefaultAsync(p => p.Id == profileId);

            if (profile == null)
            {
                throw new KeyNotFoundException("Profile not found.");
            }

            // Update the profile name
            profile.Name = updateProfileDTO.Name;

            // Retrieve and update the address based on the AddressId in the profile
            var address = await _context.Set<Address>()
                .FirstOrDefaultAsync(a => a.Id == profile.AddressId);

            if (address != null)
            {
                address.StreetAndNumber = updateProfileDTO.StreetAndNumber;
                address.City = updateProfileDTO.City;
                address.PostalCode = updateProfileDTO.PostalCode;
            }
            else
            {
                throw new KeyNotFoundException("Address not found.");
            }

            // Retrieve and update the contact based on the ContactId in the profile
            var contact = await _context.Set<Contact>()
                .FirstOrDefaultAsync(c => c.Id == profile.ContactId);

            if (contact != null)
            {
                contact.Email = updateProfileDTO.Email;
                contact.PhoneNumber = updateProfileDTO.PhoneNumber;
            }
            else
            {
                throw new KeyNotFoundException("Contact not found.");
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }


        public async Task<UpdateProfileDTO> GetUpdateProfileByIdAsync(int id)
        {
            var profile = await _context.Profiles
                .Where(p => p.Id == id)
                .Join(_context.Addresses, p => p.AddressId, a => a.Id, (p, a) => new { p, a }) 
                .Join(_context.Contacts, pa => pa.p.ContactId, c => c.Id, (pa, c) => new { pa.p, pa.a, c })  
                .Select(x => new UpdateProfileDTO
                {
                    Id = x.p.Id,
                    Name = x.p.Name,
                    StreetAndNumber = x.a.StreetAndNumber,
                    City = x.a.City,
                    PostalCode = x.a.PostalCode,
                    Email = x.c.Email,
                    PhoneNumber = x.c.PhoneNumber
                })
                .FirstOrDefaultAsync();

            if (profile == null)
            {
                throw new KeyNotFoundException("Profile not found");
            }
            return profile;
        }

        public async Task DeleteProfileByIdAsync(int profileId)
        {
            try
            {
                var profile = await _context.Profiles
                    .Where(p => p.Id == profileId)
                    .Select(p => new
                    {
                        Profile = p,
                        p.UserId,
                        AddressId = p.AddressId,
                        ContactId = p.ContactId
                    })
                    .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Profile with ID {profileId} not found.");

                var userProfileCount = await _context.Profiles.CountAsync(p => p.UserId == profile.UserId);

                if (userProfileCount <= 1)
                {
                    throw new InvalidOperationException("Cannot delete the only profile for the user.");
                }

                _context.Profiles.Remove(profile.Profile);
                await _context.SaveChangesAsync();

                var address = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == profile.AddressId);
                var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == profile.ContactId);

                if (address != null)
                {
                    _context.Addresses.Remove(address);
                }

                if (contact != null)
                {
                    _context.Contacts.Remove(contact);
                }

                await _context.SaveChangesAsync();
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception($"Operation failed: {ex.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting profile: {ex.Message}");
            }
        }

    }
}
