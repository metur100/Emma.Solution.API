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

        public async Task<User?> GetProfileByEmailAndPasswordAsync(string email, string hashedPassword)
        {
            return await _dbContext.Users
                .SingleOrDefaultAsync(p => p.Email == email && p.Password == hashedPassword);
        }

        public async Task AddAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
