using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.VehicleHandler
{
    public class VehicleHandler(IVehicleRepository vehicleRepository) : IVehicleHandler
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

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

        public async Task<List<VehicleListDTO>> GetAllVehiclesAsync()
        {
            var allVehicles = await _vehicleRepository.GetAllVehiclesAsync();
            return allVehicles ?? throw new Exception("No vehicles found.");
        }

        public async Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int userId, int vehicleId)
        {
            return await _vehicleRepository.GetVehicleInformationByVehicleIdAsync(userId, vehicleId)
                   ?? throw new Exception($"Vehicle with ID {vehicleId} for user {userId} not found.");
        }

    }
}
