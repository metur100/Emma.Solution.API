using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.UserRepository;

namespace Emma.Api.Handlers.UserHandler
{
    public class UserHandler(IUserRepository userRepository,
        IAddressRepository addressRepository,
        IContactRepository contactRepository) : IUserHandler
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IContactRepository _contactRepository = contactRepository;

        public async Task<int> CreateUserAsync(string name, string? profilePicture, string streetAndNumber, string postalCode, string city, string email, string phoneNumber)
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

            var user = new User
            {
                Name = name,
                ProfilePicture = profilePicture,
                AddressId = addressId,
                ContactId = contactId
            };
            return await _userRepository.CreateUserAsync(user);
        }

        public async Task<List<UserDto>> GetUserNamesAsync()
        {
            return await _userRepository.GetUserNamesAsync();
        }
    }
}
