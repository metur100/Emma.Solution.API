using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.VehicleRepository
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(int profileId, int vehicleId);
        Task<List<Vehicle>> GetVehiclesByProfileIdsAsync(List<int> profileIds);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task<Vehicle?> GetVehicleByIdAsync(int id);
        Task DeleteVehicleAsync(Vehicle vehicle);
        Task<int?> GetProfileIdByVehicleIdAsync(int vehicleId);
        Task AddVehicleAsync(Vehicle vehicle);
        Task<List<int>> GetVehicleIdsByProfileIdsAsync(IEnumerable<int> profileIds);
        Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId);
        Task<int> GetVehicleCountAsync();
        Task<List<BodyTypeCountDTO>> GetBodyTypeCountsAsync();
        Task<List<Vehicle>> SearchVehiclesAsync(string searchTerm);
    }
}
