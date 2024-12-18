using Emma.Api.Models;
using Emma.Api.Models.DTOs;
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

        public async Task<int> GetVehicleCountAsync()
        {
            return await _context.Vehicles.CountAsync();
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

        public async Task<List<BodyTypeCountDTO>> GetBodyTypeCountsAsync()
        {
            var bodyTypeCounts = await _context.Vehicles
                .GroupBy(v => v.BodyType)
                .Select(group => new BodyTypeCountDTO
                {
                    BodyType = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return bodyTypeCounts;
        }

        public async Task<List<Vehicle>> SearchVehiclesAsync(string searchTerm)
        {
            // Retrieve all vehicles from the database first (without filtering)
            var vehicles = await _context.Vehicles.ToListAsync();

            // Perform client-side filtering after retrieving data
            var filteredVehicles = vehicles.Where(v =>
                (v.Brand != null && v.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.PlateNumber != null && v.PlateNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.Description != null && v.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.BodyType != null && v.BodyType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.PayloadCapacity != null && v.PayloadCapacity.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.CargoSpaceVolume != null && v.CargoSpaceVolume.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.Length != null && v.Length.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.Weight != null && v.Weight.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return filteredVehicles;
        }

    }
}
