using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.UserRepository
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<int> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }

        public async Task<List<UserDto>> GetUserNamesAsync()
        {
            return await _context.Users
                .Select(u => new UserDto { Id = u.Id, Name = u.Name })
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
    }
}
