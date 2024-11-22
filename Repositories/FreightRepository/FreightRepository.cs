using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.UserRepository;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<FreightListDTO>> GetAllFreightsAsync()
        {
            try
            {
                var freights = await _context.Freights
                    .Select(freight => new FreightListDTO
                    {
                        Id = freight.Id,
                        UserName = _context.Users
                            .Where(user => user.Id == freight.UserId)
                            .Select(user => user.Name)
                            .FirstOrDefault() ?? "Unknown User",
                        LoadingAddress = freight.LoadingAddress,
                        AvailabilityDate = freight.AvailabilityDate
                    })
                    .ToListAsync();

                return freights;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all freights: {ex.Message}");
            }
        }
    }
}
