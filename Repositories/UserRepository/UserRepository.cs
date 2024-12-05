using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.UserRepository
{
    public class UserRepository(AppDbContext dbContext) : IUserRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<User?> GetProfileByEmailAsync(string email)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(p => p.Email == email);
        }

        public async Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword)
        {
            return await _dbContext.Users
                .SingleOrDefaultAsync(p => p.Email == email && p.Password == hashedPassword);
        }

        public async Task RegisterUserAsync(User user, Address address, Contact contact, string userName)
        {
            _dbContext.Addresses.Add(address);
            _dbContext.Contacts.Add(contact);
            await _dbContext.SaveChangesAsync();

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profile = new Profile
            {
                Name = userName, 
                AddressId = address.Id,
                ContactId = contact.Id,
                UserId = user.Id,
                
            };

            _dbContext.Profiles.Add(profile);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int?> GetProfileIdByUserIdAsync(int userId)
        {
            var profile = await _dbContext.Profiles
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return profile?.Id; 
        }
    }
}
