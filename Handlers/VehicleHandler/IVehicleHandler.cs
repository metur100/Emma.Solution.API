using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.VehicleHandler
{
    public interface IVehicleHandler
    {
        Task CreateVehicleAsync(Vehicle vehicle, int profileId);
        Task<List<VehicleListDTO>> GetAllVehiclesAsync();
        Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int userId, int vehicleId);
        Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int userId);
    }
}
