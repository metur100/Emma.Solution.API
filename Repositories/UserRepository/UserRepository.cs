using Emma.Api.Models;
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

        public async Task AddProfileAsync(User profile)
        {
            _dbContext.Users.Add(profile);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword)
        {
            return await _dbContext.Users
                .SingleOrDefaultAsync(p => p.Email == email && p.Password == hashedPassword);
        }
    }
}
