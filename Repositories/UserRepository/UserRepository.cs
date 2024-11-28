using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.UserRepository
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<int> CreateUserAsync(Profile user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<List<UserDto>> GetUserNamesAsync()
        {
            return await _context.Users
                .Select(u => new UserDto { Id = u.Id, UserName = u.Name })
                .ToListAsync();
        }

        public async Task<int> GetUserById(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();

            return user == null ? throw new Exception("User not found") : user.Id;
        }

        public async Task<string> GetUserNameById(string userName)
        {
            var user = await _context.Users
                .Where(u => u.Name == userName)
                .FirstOrDefaultAsync();

            return user == null ? throw new Exception("User not found") : user.Name;
        }

        public async Task<UserProfileDTO> GetUserProfileInformations(int id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    User = u,
                    Address = _context.Addresses.FirstOrDefault(a => a.Id == u.AddressId),
                    Contact = _context.Contacts.FirstOrDefault(c => c.Id == u.ContactId)
                })
                .FirstOrDefaultAsync() ?? throw new Exception("User not found");

            var userProfile = new UserProfileDTO
            {
                Id = user.User.Id,
                UserName = user.User.Name,
                ProfilePicture = user.User.ProfilePicture,
                StreetAndNumber = user.Address?.StreetAndNumber ?? string.Empty, 
                PostalCode = user.Address?.PostalCode ?? string.Empty,
                City = user.Address?.City ?? string.Empty,
                Email = user.Contact?.Email ?? string.Empty,
                PhoneNumber = user.Contact?.PhoneNumber ?? string.Empty,
                Ratings = user.User.Ratings
            };

            return userProfile;
        }
    }
}
