using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.VehicleHandler
{
    public class VehicleHandler(IVehicleRepository vehicleRepository,
        IProfileRepository profileRepository,
        IContactRepository contactRepository,
        IAddressRepository addressRepository) : IVehicleHandler
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;

        public async Task CreateVehicleAsync(Vehicle vehicle, int profileId)
        {
            try
            {
                var userProfileId = await _profileRepository.GetProfileIdByIdAsync(profileId);

                if (userProfileId == 0)
                {
                    throw new Exception($"Profile with ID {profileId} not found.");
                }
                vehicle.ProfileId = userProfileId;

                await _vehicleRepository.AddVehicleAsync(vehicle);
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
                var vehicles = await _vehicleRepository.GetAllVehiclesAsync();
                var vehicleList = new List<VehicleListDTO>();

                foreach (var vehicle in vehicles)
                {
                    var profile = await _profileRepository.GetProfileByIdAsync(vehicle.ProfileId);
                    var userName = profile?.Name ?? "Unknown User";
                    var profilePicture = profile?.ProfilePicture ?? "Unknown Picture";

                    var vehicleDto = new VehicleListDTO
                    {
                        Id = vehicle.Id,
                        ProfileId = vehicle.ProfileId,
                        UserName = userName,
                        ProfilePicture = profilePicture,
                        BodyType = vehicle.BodyType,
                        PayloadCapacity = vehicle.PayloadCapacity ?? "Not Specified"
                    };

                    vehicleList.Add(vehicleDto);
                }

                return vehicleList;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all vehicles: {ex.Message}");
            }
        }

        public async Task<VehicleInformationsDTO> GetVehicleInformationByVehicleIdAsync(int profileId, int vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(profileId, vehicleId) 
                    ?? throw new Exception($"Vehicle with ID {vehicleId} for profile {profileId} not found.");

                var profile = await _profileRepository.GetProfileByIdAsync(profileId) 
                    ?? throw new Exception($"Profile with ID {profileId} not found.");

                var address = await _addressRepository.GetAddressByIdAsync(profile.AddressId) 
                    ?? throw new Exception($"Address not found for profile {profileId}.");

                var contact = await _contactRepository.GetContactByIdAsync(profile.ContactId) 
                    ?? throw new Exception($"Contact not found for profile {profileId}.");

                var vehicleInfo = new VehicleInformationsDTO
                {
                    Id = vehicle.Id,
                    ProfileId = vehicle.ProfileId,
                    UserName = profile.Name,
                    StreetAndNumber = address.StreetAndNumber,
                    PostalCode = address.PostalCode,
                    City = address.City,
                    Email = contact.Email,
                    PhoneNumber = contact.PhoneNumber,
                    Brand = vehicle.Brand,
                    PlateNumber = vehicle.PlateNumber,
                    Description = vehicle.Description,
                    BodyType = vehicle.BodyType,
                    PayloadCapacity = vehicle.PayloadCapacity,
                    CargoSpaceVolume = vehicle.CargoSpaceVolume,
                    Length = vehicle.Length,
                    Weight = vehicle.Weight
                };

                return vehicleInfo;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vehicle information: {ex.Message}");
            }
        }

        public async Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId)
        {
            try
            {
                var vehicles = await _vehicleRepository.GetVehiclesByProfileIdAsync(profileId);

                if (vehicles == null || !vehicles.Any())
                {
                    throw new Exception($"No vehicles found for profile {profileId}.");
                }

                return vehicles;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vehicles for user {profileId}: {ex.Message}");
            }
        }


        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _vehicleRepository.GetVehicleByIdAsync(id);
        }

        public async Task<IEnumerable<VehicleManagementDTO>> GetVehiclesByUserIdAsync(int userId)
        {
            return await _vehicleRepository.GetVehiclesByUserIdAsync(userId);
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            var existingVehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicle.Id);
            if (existingVehicle == null)
                throw new KeyNotFoundException($"Vehicle with ID {vehicle.Id} not found.");

            existingVehicle.Brand = vehicle.Brand;
            existingVehicle.PlateNumber = vehicle.PlateNumber;
            existingVehicle.Description = vehicle.Description;
            existingVehicle.BodyType = vehicle.BodyType;
            existingVehicle.PayloadCapacity = vehicle.PayloadCapacity;
            existingVehicle.CargoSpaceVolume = vehicle.CargoSpaceVolume;
            existingVehicle.Length = vehicle.Length;
            existingVehicle.Weight = vehicle.Weight;


            await _vehicleRepository.UpdateVehicleAsync(existingVehicle);
        }

        public async Task DeleteVehicleByIdAsync(int id)
        {
            await _vehicleRepository.DeleteVehicleByIdAsync(id);
        }

        public async Task<string?> GetProfileNameByVehicleIdAsync(int vehicleId)
        {
            return await _vehicleRepository.GetProfileNameByVehicleIdAsync(vehicleId);
        }
    }
}
