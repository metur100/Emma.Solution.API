using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.ProfileRepository;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.VehicleRepository
{
    public class VehicleRepository(AppDbContext context, IProfileRepository profileRepository) : IVehicleRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IProfileRepository _profileRepository = profileRepository;

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

        public async Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId)
        {
            return await _context.Vehicles
                .Where(vehicle => vehicle.ProfileId == profileId)
                .ToListAsync();
        }


        public async Task<IEnumerable<VehicleManagementDTO>> GetVehiclesByUserIdAsync(int userId)
        {
            var userProfileIds = await _context.Set<Profile>()
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            if (!userProfileIds.Any())
            {
                return Enumerable.Empty<VehicleManagementDTO>();
            }

            return await _context.Set<Vehicle>()
                .Where(f => userProfileIds.Contains(f.ProfileId))
                .Select(f => new VehicleManagementDTO
                {
                    Id = f.Id,
                    Brand = f.Brand,
                    BodyType = f.BodyType,
                    PayloadCapacity = f.PayloadCapacity
                })
                .ToListAsync();
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Set<Vehicle>().Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles
                .Where(freight => freight.Id == id)
                .FirstOrDefaultAsync() ?? throw new Exception("");
        }

        public async Task DeleteVehicleByIdAsync(int id)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (vehicle == null)
                {
                    throw new Exception($"Vehicle with ID {id} not found.");
                }

                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting vehicle: {ex.Message}");
            }
        }

        public async Task<string?> GetProfileNameByVehicleIdAsync(int vehicleId)
        {
            try
            {
                var profileName = await (from vehicle in _context.Vehicles
                                         join profile in _context.Profiles on vehicle.ProfileId equals profile.Id
                                         where vehicle.Id == vehicleId
                                         select profile.Name)
                                     .FirstOrDefaultAsync();

                return profileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching Profile Name for VehicleId {vehicleId}: {ex.Message}");
            }
        }
    }
}
