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

        public async Task<int> CreateProfileAsync(string name, string? profilePicture, string streetAndNumber, string postalCode, string city, string email, string phoneNumber)
        {
            var address = new Address
            {
                StreetAndNumber = streetAndNumber,
                PostalCode = postalCode,
                City = city
            };
            var addressId = await _addressRepository.CreateAddress(address);

            var contact = new Contact
            {
                Email = email,
                PhoneNumber = phoneNumber
            };
            var contactId = await _contactRepository.CreateContact(contact);

            var user = new Profile
            {
                Name = name,
                UserId = 1,
                ProfilePicture = profilePicture,
                AddressId = addressId,
                ContactId = contactId
            };
            return await _profileRepository.CreateProfileAsync(user);
        }

        public async Task<List<ProfileDto>> GetProfileNamesAsync()
        {
            return await _profileRepository.GetProfileNamesAsync();
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

        public async Task<string> GetProfileNameAsync(int id)
        {
            return await _profileRepository.GetProfileNameById(id);
        }

    }
}
