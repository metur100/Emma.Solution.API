using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.VehicleRepository;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _vehicleRepository.GetVehicleByIdAsync(id);
        }

        public async Task<IEnumerable<VehicleManagementDTO>> GetVehiclesByUserIdAsync(int userId)
        {
            return await _vehicleRepository.GetVehiclesByUserIdAsync(userId);
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            var existingVehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicle.Id);
            if (existingVehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {vehicle.Id} not found.");

            existingVehicle.Brand = vehicle.Brand;
            existingVehicle.PlateNumber = vehicle.PlateNumber;
            existingVehicle.Description = vehicle.Description;
            existingVehicle.BodyType = vehicle.BodyType;
            existingVehicle.PayloadCapacity = vehicle.PayloadCapacity;
            existingVehicle.CargoSpaceVolume = vehicle.CargoSpaceVolume;
            existingVehicle.Length = vehicle.Length;
            existingVehicle.Weight = vehicle.Weight;


            await _vehicleRepository.UpdateVehicleAsync(existingVehicle);
        }

        public async Task DeleteVehicleByIdAsync(int id)
        {
            await _vehicleRepository.DeleteVehicleByIdAsync(id);
        }

        public async Task<string?> GetProfileNameByVehicleIdAsync(int vehicleId)
        {
            return await _vehicleRepository.GetProfileNameByVehicleIdAsync(vehicleId);
        }
    }
}
