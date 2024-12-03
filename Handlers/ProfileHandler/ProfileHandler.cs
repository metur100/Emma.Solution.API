using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.ProfileRepository;

namespace Emma.Api.Handlers.ProfileHandler
{
    public class ProfileHandler(IProfileRepository userRepository,
        IAddressRepository addressRepository,
        IContactRepository contactRepository) : IProfileHandler
    {
        private readonly IProfileRepository _profileRepository = userRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IContactRepository _contactRepository = contactRepository;

        public async Task<int> CreateProfileAsync(
            string name,
            string? profilePicture,
            string streetAndNumber,
            string postalCode,
            string city,
            string email,
            string phoneNumber,
            int userId)
        {
            // Create and save the address
            var address = new Address
            {
                StreetAndNumber = streetAndNumber,
                PostalCode = postalCode,
                City = city
            };
            var addressId = await _addressRepository.CreateAddress(address);

            // Create and save the contact
            var contact = new Contact
            {
                Email = email,
                PhoneNumber = phoneNumber
            };
            var contactId = await _contactRepository.CreateContact(contact);

            // Create the profile with the provided userId
            var profile = new Profile
            {
                Name = name,
                ProfilePicture = profilePicture,
                AddressId = addressId,
                ContactId = contactId,
                UserId = userId
            };

            // Call the repository method, passing the userId
            return await _profileRepository.CreateProfileAsync(profile, userId);
        }

        public async Task<List<ProfileDto>> GetProfileNamesByUserIdAsync(int userId)
        {
            try
            {
                return await _profileRepository.GetProfileNamesByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the user profiles for user ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<ProfileInfoDTO> GetUserProfileInformations(int id)
        {
            try
            {
                return await _profileRepository.GetUserProfileInformations(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the user profile for user ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<string> GetProfileNameByUserId(int userId)
        {
            try
            {
                return await _profileRepository.GetProfileNameByUserId(userId);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while retrieving the user profile for user ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ProfileManagementDTO>> GetProfilesByUserIdAsync(int userId)
        {
            return await _profileRepository.GetProfilesByUserIdAsync(userId);
        }

        public async Task UpdateProfileAsync(int profileId, UpdateProfileDTO updateProfileDTO)
        {
            await _profileRepository.UpdateProfileAsync(profileId, updateProfileDTO);
        }

        public async Task<UpdateProfileDTO> HandleGetProfileByIdAsync(int id)
        {
            return await _profileRepository.GetProfileByIdAsync(id);
        }

        public async Task HandleDeleteProfileByIdAsync(int id)
        {
            await _profileRepository.DeleteProfileByIdAsync(id);
        }
    }
}
