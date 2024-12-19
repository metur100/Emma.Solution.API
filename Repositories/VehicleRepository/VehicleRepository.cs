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

        public async Task<List<VehicleSearchDTO>> SearchVehiclesAsync(string searchTerm)
        {
            // First, retrieve the data from the database
            var vehiclesWithProfileName = await (from v in _context.Vehicles
                                                 join p in _context.Profiles on v.ProfileId equals p.Id
                                                 select new
                                                 {
                                                     Vehicle = v,
                                                     ProfileName = p.Name
                                                 }).ToListAsync();

            // Now, perform the filtering client-side
            var filteredVehicles = vehiclesWithProfileName
                .Where(x =>
                    (x.ProfileName != null && x.ProfileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) || // Filter by ProfileName
                    (x.Vehicle.Brand != null && x.Vehicle.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.PlateNumber != null && x.Vehicle.PlateNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.Description != null && x.Vehicle.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.BodyType != null && x.Vehicle.BodyType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.PayloadCapacity != null && x.Vehicle.PayloadCapacity.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.CargoSpaceVolume != null && x.Vehicle.CargoSpaceVolume.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.Length != null && x.Vehicle.Length.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Vehicle.Weight != null && x.Vehicle.Weight.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new VehicleSearchDTO
                {
                    Id = x.Vehicle.Id,
                    Brand = x.Vehicle.Brand,
                    PlateNumber = x.Vehicle.PlateNumber,
                    Description = x.Vehicle.Description,
                    BodyType = x.Vehicle.BodyType,
                    PayloadCapacity = x.Vehicle.PayloadCapacity,
                    CargoSpaceVolume = x.Vehicle.CargoSpaceVolume,
                    Length = x.Vehicle.Length,
                    Weight = x.Vehicle.Weight,
                    ProfileId = x.Vehicle.ProfileId,
                    ProfileName = x.ProfileName // Include ProfileName from the joined Profile
                })
                .ToList();

            return filteredVehicles;
        }

    }
}
