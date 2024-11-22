using Emma.Api.Models;

namespace Emma.Api.Repositories.VehicleRepository
{
    public interface IVehicleRepository
    {
        Task CreateVehicleAsync(Vehicle vehicle, int userId);
    }
}
