using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.StatusRepository;

namespace Emma.Api.Handlers.FreightHandler
{
    public class FreightHandler(IFreightRepository freightRepository, 
        IProfileRepository profileRepository,
        IContactRepository contactRepository,
        IAddressRepository addressRepository,
        IOfferRepository offerRepository,
        IStatusRepository statusRepository) : IFreightHandler
    {
        private readonly IFreightRepository _freightRepository = freightRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IOfferRepository _offerRepository = offerRepository;
        private readonly IStatusRepository _statusRepository = statusRepository;

        public async Task CreateFreightAsync(Freight freight, int profileId)
        {
            try
            {
                var profile = await _profileRepository.GetProfileIdById(profileId);
                freight.ProfileId = profile;
                await _freightRepository.AddFreightAsync(freight);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating freight: {ex.Message}");
            }
        }

        public async Task<List<FreightListDTO>> GetAllFreightsAsync()
        {
            try
            {
                var freights = await _freightRepository.GetAllFreightsAsync();
                var freightListDTOs = new List<FreightListDTO>();

                foreach (var freight in freights)
                {
                    var profile = await _profileRepository.GetProfileByIdAsync(freight.ProfileId);
                    var profileName = profile?.Name ?? "Unknown User";
                    var profilePicture = profile?.ProfilePicture ?? "Unknown Picture";

                    freightListDTOs.Add(new FreightListDTO
                    {
                        Id = freight.Id,
                        ProfileId = freight.ProfileId,
                        ProfileName = profileName,
                        ProfilePicture = profilePicture,
                        LoadingAddress = freight.LoadingAddress,
                        AvailabilityDate = freight.AvailabilityDate
                    });
                }

                return freightListDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all freights: {ex.Message}");
            }
        }

        public async Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int profileId, int freightId)
        {
            try
            {
                var freight = await _freightRepository.GetFreightByProfileAndIdAsync(profileId, freightId) ?? throw new Exception($"Freight with ID {freightId} for user {profileId} not found.");
                var profile = await _profileRepository.GetProfileByIdAsync(profileId) ?? throw new Exception($"User with ID {profileId} not found.");
                var address = await _addressRepository.GetAddressByIdAsync(profile.AddressId);
                var contact = await _contactRepository.GetContactByIdAsync(profile.ContactId);

                return new FreightInformationsDTO
                {
                    Id = freight.Id,
                    ProfileId = freight.ProfileId,
                    ProfileName = profile.Name,
                    StreetAndNumber = address?.StreetAndNumber ?? "Unknown Address",
                    PostalCode = address?.PostalCode ?? "Unknown Postal Code",
                    City = address?.City ?? "Unknown City",
                    Email = contact?.Email ?? "Unknown Email",
                    PhoneNumber = contact?.PhoneNumber ?? "Unknown Phone Number",
                    LoadingAddress = freight.LoadingAddress,
                    UnloadingAddress = freight.UnloadingAddress,
                    TransportPrice = freight.TransportPrice,
                    Status = freight.Status,
                    Distance = freight.Distance,
                    GoodsDescription = freight.GoodsDescription,
                    GoodsType = freight.GoodsType,
                    GoodsWeight = freight.GoodsWeight,
                    GoodsLength = freight.GoodsLength,
                    GoodsPrice = freight.GoodsPrice,
                    AvailabilityDate = freight.AvailabilityDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching freight information: {ex.Message}");
            }
        }

        public async Task<Freight?> GetFreightByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("The freight ID must be a positive integer.");

            try
            {
                var freight = await _freightRepository.GetFreightByIdAsync(id);

                if (freight == null)
                    throw new KeyNotFoundException($"No freight found with ID {id}.");

                return freight;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving the freight with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<FreightManagementDTO>> GetFreightsByUserIdAsync(int userId)
        {
            try
            {
                var userProfileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);
                if (!userProfileIds.Any())
                {
                    return Enumerable.Empty<FreightManagementDTO>();
                }

                var freights = await _freightRepository.GetFreightsByProfileIdsAsync(userProfileIds);

                return freights.Select(f => new FreightManagementDTO
                {
                    Id = f.Id,
                    GoodsDescription = f.GoodsDescription,
                    GoodsWeight = f.GoodsWeight,
                    Distance = f.Distance
                }).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching freights for user {userId}: {ex.Message}");
            }
        }

        public async Task UpdateFreightAsync(Freight freight)
        {
            if (freight == null)
                throw new ArgumentNullException(nameof(freight), "Freight cannot be null.");

            if (freight.Id <= 0)
                throw new ArgumentException("Invalid freight ID.", nameof(freight.Id));

            try
            {
                var existingFreight = await _freightRepository.GetFreightByIdAsync(freight.Id)
                    ?? throw new KeyNotFoundException($"Freight with ID {freight.Id} not found.");

                existingFreight.LoadingAddress = freight.LoadingAddress;
                existingFreight.UnloadingAddress = freight.UnloadingAddress;
                existingFreight.TransportPrice = freight.TransportPrice;
                existingFreight.Status = freight.Status;
                existingFreight.Distance = freight.Distance;
                existingFreight.GoodsDescription = freight.GoodsDescription;
                existingFreight.GoodsType = freight.GoodsType;
                existingFreight.GoodsWeight = freight.GoodsWeight;
                existingFreight.GoodsLength = freight.GoodsLength;
                existingFreight.GoodsPrice = freight.GoodsPrice;
                existingFreight.AvailabilityDate = freight.AvailabilityDate;

                await _freightRepository.UpdateFreightAsync(existingFreight);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while updating the freight.", ex);
            }
        }

        public async Task DeleteFreightByIdAsync(int freightId)
        {
            try
            {
                var freight = await _freightRepository.GetFreightByIdAsync(freightId);
                if (freight == null)
                {
                    throw new Exception($"Vehicle with ID {freightId} not found.");
                }

                var offers = await _offerRepository.GetOffersByFreightIdAsync(freightId);
                if (offers.Any())
                {
                    var offerIds = offers.Select(o => o.Id).ToList();
                    await _statusRepository.DeleteStatusesByOfferIdsAsync(offerIds);

                    await _offerRepository.DeleteOffersByFreightIdAsync(freightId);
                }
                await _freightRepository.DeleteFreightAsync(freight);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting vehicle and its associated data: {ex.Message}");
            }
        }

        public async Task<int?> GetProfileIdsByFreightIdAsync(int freightId)
        {
            try
            {
                var profileId = await _freightRepository.GetProfileIdsByFreightIdAsync(freightId) 
                    ?? throw new Exception($"Profile name not found for FreightId {freightId}");
                return profileId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching Profile Name for FreightId {freightId}: {ex.Message}");
            }
        }

        public async Task<List<Freight>> GetFreightsByProfileIdAsync(int profileId)
        {
            try
            {
                var freights = await _freightRepository.GetFreightsByProfileIdAsync(profileId)
                    ?? throw new Exception($"Freights with {profileId} not found.");
                return freights;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching freights with {profileId}: {ex.Message}", ex);
            }
        }

        public async Task<int> GetAllFreightsCountAsync()
        {
            try
            {
                return await _freightRepository.GetFreightsCountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all freights: {ex.Message}");
            }
        }
    }
}
