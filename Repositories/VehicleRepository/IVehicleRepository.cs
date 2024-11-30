using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.VehicleRepository
{
    public interface IVehicleRepository
    {
        Task CreateVehicleAsync(Vehicle vehicle, int profileId);
        Task<List<VehicleListDTO>> GetAllVehiclesAsync();
        Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int profileId, int vehicleId);
        Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId);
    }
}
