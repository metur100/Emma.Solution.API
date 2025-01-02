using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.StatusRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.VehicleHandler
{
    public class VehicleHandler(IVehicleRepository vehicleRepository,
        IProfileRepository profileRepository,
        IContactRepository contactRepository,
        IAddressRepository addressRepository,
        IOfferRepository offerRepository,
        IStatusRepository statusRepository) : IVehicleHandler
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IOfferRepository _offerRepository = offerRepository;
        private readonly IStatusRepository _statusRepository = statusRepository;

        public async Task CreateVehicleAsync(Vehicle vehicle, int profileId)
        {
            try
            {
                var userProfileId = await _profileRepository.GetProfileIdById(profileId);

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
                var activeVehicles = vehicles.Where(v => v.IsActive).ToList();
                var vehicleList = new List<VehicleListDTO>();

                foreach (var vehicle in activeVehicles)
                {
                    var profile = await _profileRepository.GetProfileByIdAsync(vehicle.ProfileId);
                    var profileName = profile?.Name ?? "Unknown User";
                    var profilePicture = profile?.ProfilePicture ?? "Unknown Picture";

                    var vehicleDto = new VehicleListDTO
                    {
                        Id = vehicle.Id,
                        ProfileId = vehicle.ProfileId,
                        ProfileName = profileName,
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
                    ProfileName = profile.Name,
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

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(id) 
                    ?? throw new Exception($"Vehicle with ID {id} not found.");
                return vehicle;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vehicle with ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<List<VehicleManagementDTO>> GetVehiclesByUserIdAsync(int userId)
        {
            var userProfileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);

            if (userProfileIds == null || !userProfileIds.Any())
            {
                return [];
            }

            var vehicles = await _vehicleRepository.GetVehiclesByProfileIdsAsync(userProfileIds);

            var vehicleDtos = vehicles.Select(v => new VehicleManagementDTO
            {
                Id = v.Id,
                Brand = v.Brand,
                BodyType = v.BodyType,
                PayloadCapacity = v.PayloadCapacity,
                PlateNumber = v.PlateNumber,
                IsActive = v.IsActive,
            }).ToList();

            return vehicleDtos;
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            if (vehicle == null)
                throw new ArgumentNullException(nameof(vehicle));

            if (vehicle.Id <= 0)
                throw new ArgumentException("Invalid vehicle ID.");

            try
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
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the vehicle.", ex);
            }
        }

        public async Task DeleteVehicleByIdAsync(int vehicleId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
                if (vehicle == null)
                {
                    throw new Exception($"Vehicle with ID {vehicleId} not found.");
                }

                var offers = await _offerRepository.GetOffersByVehicleIdAsync(vehicleId);
                if (offers.Any())
                {
                    var offerIds = offers.Select(o => o.Id).ToList();
                    await _statusRepository.DeleteStatusesByOfferIdsAsync(offerIds); 

                    await _offerRepository.DeleteOffersByVehicleIdAsync(vehicleId);
                }
                await _vehicleRepository.DeleteVehicleAsync(vehicle);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting vehicle and its associated data: {ex.Message}");
            }
        }

        public async Task<int?> GetProfileIdByVehicleIdAsync(int vehicleId)
        {
            try
            {
                var profileId = await _vehicleRepository.GetProfileIdByVehicleIdAsync(vehicleId);

                return profileId == null ? throw new Exception($"Profile id not found for vehicle with ID {vehicleId}.") : profileId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching Profile id for VehicleId {vehicleId}: {ex.Message}");
            }
        }

        public async Task<List<Vehicle>> GetVehiclesByProfileIdAsync(int profileId)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetVehiclesByProfileIdAsync(profileId)
                    ?? throw new Exception($"Vehicle with {profileId} not found.");
                return vehicle;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching vehicle with {profileId}: {ex.Message}", ex);
            }
        }

        public async Task<int> GetAllVehiclesCountAsync()
        {
            try
            {
                return await _vehicleRepository.GetVehicleCountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all vehicles: {ex.Message}");
            }
        }

        public async Task<List<BodyTypeCountDTO>> GetAllBodyTypesCountsAsync()
        {
            try
            {
                return await _vehicleRepository.GetBodyTypeCountsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all body types: {ex.Message}");
            }
        }

        public async Task<List<VehicleSearchDTO>> HandleSearchVehiclesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                throw new ArgumentException("Search term cannot be empty or null.", nameof(searchTerm));
            }

            var vehicles = await _vehicleRepository.SearchVehiclesAsync(searchTerm.Trim());
            return vehicles;
        }

        public async Task<List<VehicleManagementDTO>> SearchVehiclesByUserIdAsync(int userId, string searchTerm)
        {
            var userProfileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);

            if (userProfileIds == null || !userProfileIds.Any())
            {
                return new List<VehicleManagementDTO>();
            }

            var vehicles = await _vehicleRepository.GetVehiclesByProfileIdsAsync(userProfileIds);

            var filteredVehicles = vehicles.Where(v =>
                (v.Brand != null && v.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.BodyType != null && v.BodyType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (v.PayloadCapacity != null && v.PayloadCapacity.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            );

            var vehicleDtos = filteredVehicles.Select(v => new VehicleManagementDTO
            {
                Id = v.Id,
                Brand = v.Brand,
                BodyType = v.BodyType,
                PayloadCapacity = v.PayloadCapacity,
                PlateNumber = v.PlateNumber
            }).ToList();

            return vehicleDtos;
        }

        public async Task HandleActiveVehicleAsync(int vehicleId)
        {
            await _vehicleRepository.ToggleVehicleStatusAsync(vehicleId);
        }
    }
}
