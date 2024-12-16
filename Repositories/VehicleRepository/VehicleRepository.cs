using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.VehicleRepository
{
    public class VehicleRepository(AppDbContext context) : IVehicleRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int profileId, int vehicleId)
        {
            return await _context.Vehicles
                .Where(v => v.ProfileId == profileId && v.Id == vehicleId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByProfileIdsAsync(List<int> profileIds)
        {
            return await _context.Set<Vehicle>()
                .Where(f => profileIds.Contains(f.ProfileId))
                .ToListAsync();
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Set<Vehicle>().Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles
                .Where(freight => freight.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetProfileIdByVehicleIdAsync(int vehicleId)
        {
            return await (from vehicle in _context.Vehicles
                          join profile in _context.Profiles on vehicle.ProfileId equals profile.Id
                          where vehicle.Id == vehicleId
                          select profile.Id)
                     .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetVehicleIdsByProfileIdsAsync(IEnumerable<int> profileIds)
        {
            return await _context.Vehicles
                .Where(v => profileIds.Contains(v.ProfileId))
                .Select(v => v.Id)
                .ToListAsync();
        }

        public async Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId)
        {
            return await _context.Set<Vehicle>()
                .Where(p => p.ProfileId == profileId)
                .ToListAsync();
        }
    }
}
