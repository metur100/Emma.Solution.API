using Emma.Api.Handlers.UserHandler;
using Emma.Api.Models;
using Emma.Api.Repositories.UserRepository;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.VehicleRepository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;  

        public VehicleRepository(AppDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task CreateVehicleAsync(Vehicle vehicle, int userId)
        {
            try
            {
                var user = await _userRepository.GetUserById(userId);  

                vehicle.UserId = user;

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating vehicle: {ex.Message}");
            }
        }
    }
}
