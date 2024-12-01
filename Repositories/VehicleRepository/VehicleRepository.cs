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

        public async Task CreateVehicleAsync(Vehicle vehicle, int profileId)
        {
            try
            {
                var user = await _profileRepository.GetProfileById(profileId);  

                vehicle.ProfileId = user;

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating vehicle: {ex.Message}");
            }
        }

        public async Task<List<VehicleListDTO>> GetAllVehiclesAsync()
        {
            try
            {
                var vehicles = await _context.Vehicles
                    .Select(vehicle => new VehicleListDTO
                    {
                        Id = vehicle.Id,
                        ProfileId  = vehicle.ProfileId,
                        UserName = _context.Profiles
                            .Where(user => user.Id == vehicle.ProfileId)
                            .Select(user => user.Name)
                            .FirstOrDefault() ?? "Unknown User",
                        ProfilePicture = _context.Profiles
                            .Where(user => user.Id == vehicle.ProfileId)
                            .Select(user => user.ProfilePicture)
                            .FirstOrDefault() ?? "Unknown Picture",
                        BodyType = vehicle.BodyType,
                        PayloadCapacity = vehicle.PayloadCapacity ?? "Not Specified"
                    })
                    .ToListAsync();

                return vehicles;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all vehicles: {ex.Message}");
            }
        }

        public async Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int profileId, int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Where(v => v.ProfileId == profileId && v.Id == vehicleId)
                .Join(_context.Profiles, vehicle => vehicle.ProfileId, profile => profile.Id, (vehicle, user) => new { vehicle, user })
                .Join(_context.Addresses, temp => temp.user.AddressId, address => address.Id, (temp, address) => new { temp.vehicle, temp.user, address })
                .Join(_context.Contacts, temp => temp.user.ContactId, contact => contact.Id, (temp, contact) => new VehicleInformationsDTO
                {
                    Id = temp.vehicle.Id,
                    ProfileId = temp.vehicle.ProfileId,
                    UserName = temp.user.Name,
                    StreetAndNumber = temp.address.StreetAndNumber,
                    PostalCode = temp.address.PostalCode,
                    City = temp.address.City,
                    Email = contact.Email,
                    PhoneNumber = contact.PhoneNumber,
                    Brand = temp.vehicle.Brand,
                    PlateNumber = temp.vehicle.PlateNumber,
                    Description = temp.vehicle.Description,
                    BodyType = temp.vehicle.BodyType,
                    PayloadCapacity = temp.vehicle.PayloadCapacity,
                    CargoSpaceVolume = temp.vehicle.CargoSpaceVolume,
                    Length = temp.vehicle.Length,
                    Weight = temp.vehicle.Weight
                })
                .FirstOrDefaultAsync();

            return vehicle ?? throw new Exception($"Vehicle with ID {vehicleId} for profile {profileId} not found.");
        }

        public async Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId)
        {
            try
            {
                var vehicles = await _context.Vehicles
                    .Where(vehicle => vehicle.ProfileId == profileId)
                    .ToListAsync();  

                return vehicles;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vehicles for user {profileId}: {ex.Message}");
            }
        }

    }
}
