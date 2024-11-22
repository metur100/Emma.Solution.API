using Emma.Api.Models;
using Emma.Api.Repositories.UserRepository;

namespace Emma.Api.Repositories.VehicleRepository
{
    public class VehicleRepository(AppDbContext context, IUserRepository userRepository) : IVehicleRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IUserRepository _userRepository = userRepository;

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
