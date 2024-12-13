using Emma.Api.Helpers;
using Emma.Api.Helpers.EmailServiceHelper;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.UserRepository;

namespace Emma.Api.Handlers.UserHandler
{
    public class UserHandler(IUserRepository userRepository, 
        IJWTHelper jWTHelper,
        IContactRepository contactRepository,
        IAddressRepository addressRepository,
        IProfileRepository profileRepository,
        IEmailServiceHelper emailServiceHelper) : IUserHandler
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJWTHelper _jWTHelper = jWTHelper;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IEmailServiceHelper _emailServiceHelper = emailServiceHelper;

        public async Task<string> RegisterUserAsync(RegisterUserDto registerUserDto)
        {
            if (registerUserDto == null)
                throw new ArgumentNullException(nameof(registerUserDto));

            if (string.IsNullOrWhiteSpace(registerUserDto.Email))
                throw new ArgumentException("Email cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(registerUserDto.Password))
                throw new ArgumentException("Password cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(registerUserDto.UserName))
                throw new ArgumentException("User name cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(registerUserDto.StreetAndNumber))
                throw new ArgumentException("Street and number cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(registerUserDto.PostalCode))
                throw new ArgumentException("Postal code cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(registerUserDto.City))
                throw new ArgumentException("City cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(registerUserDto.PhoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.");

            try
            {
                var user = new User
                {
                    Email = registerUserDto.Email,
                    Password = registerUserDto.Password,
                };

                var address = new Address
                {
                    StreetAndNumber = registerUserDto.StreetAndNumber,
                    PostalCode = registerUserDto.PostalCode,
                    City = registerUserDto.City
                };

                var contact = new Contact
                {
                    Email = registerUserDto.Email,
                    PhoneNumber = registerUserDto.PhoneNumber
                };

                await _addressRepository.CreateAddress(address);
                await _contactRepository.CreateContact(contact);
                await _userRepository.AddAsync(user);

                var profile = new Profile
                {
                    Name = registerUserDto.UserName,
                    AddressId = address.Id,
                    ContactId = contact.Id,
                    UserId = user.Id,
                };

                await _profileRepository.CreateProfileAsync(profile, user.Id);

                return "User registered successfully.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during user registration.", ex);
            }
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.");

            try
            {
                var hashedPassword = _jWTHelper.HashPassword(password);

                var user = await _userRepository.GetProfileByEmailAndPasswordAsync(email, hashedPassword);
                if (user == null)
                {
                    return "Invalid email or password.";
                }

                return await _jWTHelper.GenerateJwtTokenAsync(user);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during login.", ex);
            }
        }

        public async Task<string> RegisterUserAsynca(RegisterUserDto registerUserDto)
        {
            if (registerUserDto == null)
                throw new ArgumentNullException(nameof(registerUserDto));
            try
            {
                var user = new User
                {
                    Email = registerUserDto.Email,
                    Password = registerUserDto.Password,
                    Confirmed = false,
                    ConfirmationToken = Guid.NewGuid().ToString() 
                };

                var address = new Address
                {
                    StreetAndNumber = registerUserDto.StreetAndNumber,
                    PostalCode = registerUserDto.PostalCode,
                    City = registerUserDto.City
                };

                var contact = new Contact
                {
                    Email = registerUserDto.Email,
                    PhoneNumber = registerUserDto.PhoneNumber
                };

                await _addressRepository.CreateAddress(address);
                await _contactRepository.CreateContact(contact);
                await _userRepository.AddAsync(user);

                var profile = new Profile
                {
                    Name = registerUserDto.UserName,
                    AddressId = address.Id,
                    ContactId = contact.Id,
                    UserId = user.Id,
                };

                await _profileRepository.CreateProfileAsync(profile, user.Id);

                var confirmationUrl = $"http://localhost:5173/confirm/{user.ConfirmationToken}";
                var emailSubject = "Confirm your registration";
                var emailBody = $@"
                    <p>Hi {registerUserDto.UserName},</p>
                    <p>Thank you for registering. Please confirm your registration by clicking the link below:</p>
                    <a href='{confirmationUrl}'>Confirm Registration</a>";

                _emailServiceHelper.SendEmail(new List<string> { user.Email }, emailSubject, emailBody);

                return "User registered successfully. Please check your email to confirm your registration.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during user registration.", ex);
            }
        }

    }
}
