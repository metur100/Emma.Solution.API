using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.VehicleHandler
{
    public class VehicleHandler(IVehicleRepository vehicleRepository) : IVehicleHandler
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;

        public async Task CreateVehicleAsync(Vehicle vehicle, int profileId)
        {
            try
            {
                await _vehicleRepository.CreateVehicleAsync(vehicle, profileId);
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

        public async Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int profileId, int vehicleId)
        {
            return await _vehicleRepository.GetVehicleInformationByVehicleIdAsync(profileId, vehicleId)
                   ?? throw new Exception($"Vehicle with ID {vehicleId} for user {profileId} not found.");
        }

        public async Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId)
        {
            return await _vehicleRepository.GetVehiclesByProfileIdAsync(profileId);
        }
    }
}
