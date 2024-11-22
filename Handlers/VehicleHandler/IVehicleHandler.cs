using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.VehicleHandler
{
    public interface IVehicleHandler
    {
        Task CreateVehicleAsync(Vehicle vehicle, int userId);
        Task<List<VehicleListDTO>> GetAllVehiclesAsync();
        Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int userId, int vehicleId);
    }
}
