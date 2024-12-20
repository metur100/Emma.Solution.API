using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.VehicleHandler
{
    public interface IVehicleHandler
    {
        Task CreateVehicleAsync(Vehicle vehicle, int profileId);
        Task<List<VehicleListDTO>> GetAllVehiclesAsync();
        Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int userId, int vehicleId);
        Task<Vehicle?> GetVehicleByIdAsync(int id);
        Task<List<VehicleManagementDTO>> GetVehiclesByUserIdAsync(int userId);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleByIdAsync(int id);
        Task<int?> GetProfileIdByVehicleIdAsync(int vehicleId);
        Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId);
        Task<int> GetAllVehiclesCountAsync();
        Task<List<BodyTypeCountDTO>> GetAllBodyTypesCountsAsync();
        Task<List<VehicleSearchDTO>> HandleSearchVehiclesAsync(string searchTerm);
    }
}
