using Emma.Api.Models;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.VehicleHandler
{
    public class VehicleHandler : IVehicleHandler
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleHandler(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        public async Task CreateVehicleAsync(Vehicle vehicle, int userId)
        {
            try
            {
                await _vehicleRepository.CreateVehicleAsync(vehicle, userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in vehicle handler: {ex.Message}");
            }
        }
    }
}
