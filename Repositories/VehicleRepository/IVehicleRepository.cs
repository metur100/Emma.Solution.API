using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.VehicleRepository
{
    public interface IVehicleRepository
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle?> GetVehicleByIdAsync(int profileId, int vehicleId);
        Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId);
        Task<IEnumerable<VehicleManagementDTO>> GetVehiclesByUserIdAsync(int userId);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task DeleteVehicleByIdAsync(int id);
        Task<string?> GetProfileNameByVehicleIdAsync(int vehicleId);
        Task AddVehicleAsync(Vehicle vehicle);
    }
}
