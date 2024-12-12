using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.ProfileRepository;

namespace Emma.Api.Handlers.FreightHandler
{
    public class FreightHandler(IFreightRepository freightRepository, 
        IProfileRepository profileRepository,
        IContactRepository contactRepository,
        IAddressRepository addressRepository) : IFreightHandler
    {
        private readonly IFreightRepository _freightRepository = freightRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;

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
                    var user = await _profileRepository.GetProfileByIdAsync(freight.ProfileId);
                    var userName = user?.Name ?? "Unknown User";
                    var profilePicture = user?.ProfilePicture ?? "Unknown Picture";

                    freightListDTOs.Add(new FreightListDTO
                    {
                        Id = freight.Id,
                        ProfileId = freight.ProfileId,
                        UserName = userName,
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
                var user = await _profileRepository.GetProfileByIdAsync(profileId) ?? throw new Exception($"User with ID {profileId} not found.");
                var address = await _addressRepository.GetAddressByIdAsync(user.AddressId);
                var contact = await _contactRepository.GetContactByIdAsync(user.ContactId);

                return new FreightInformationsDTO
                {
                    Id = freight.Id,
                    ProfileId = freight.ProfileId,
                    UserName = user.Name,
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


        public async Task<Freight> GetFreightByIdAsync(int id)
        {
            return await _freightRepository.GetFreightByIdAsync(id);
        }

        public async Task<IEnumerable<FreightManagementDTO>> GetFreightsByUserAsync(int userId)
        {
            return await _freightRepository.GetFreightsByUserIdAsync(userId);
        }

        public async Task UpdateFreightAsync(Freight freight)
        {
            var existingFreight = await _freightRepository.GetFreightByIdAsync(freight.Id);
            if (existingFreight == null)
                throw new KeyNotFoundException($"Freight with ID {freight.Id} not found.");

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

        public async Task DeleteFreightByIdAsync(int id)
        {
            await _freightRepository.DeleteFreightByIdAsync(id);
        }

        public async Task<string?> GetProfileNameByFreightIdAsync(int freightId)
        {
            return await _freightRepository.GetProfileNameByFreightIdAsync(freightId);
        }
    }
}
