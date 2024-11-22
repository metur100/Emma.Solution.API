using Emma.Api.Models;
using Emma.Api.Repositories.UserRepository;

namespace Emma.Api.Repositories.FreightRepository
{
    public class FreightRepository(AppDbContext context, IUserRepository userRepository) : IFreightRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task CreateFreightAsync(Freight freight, int userId)
        {
            try
            {
                var user = await _userRepository.GetUserById(userId);

                freight.UserId = user;

                _context.Freights.Add(freight);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating freight: {ex.Message}");
            }
        }
    }
}
