using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.UserRepository;

namespace Emma.Api.Handlers.UserHandler
{
    public class UserHandler(IUserRepository userRepository, IJWTHelper jWTHelper) : IUserHandler
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IJWTHelper _jWTHelper = jWTHelper;

        public async Task<string> RegisterAsync(RegisterUserDto dto)
        {
            var existingProfile = await _userRepository.GetProfileByEmailAsync(dto.Email);
            if (existingProfile != null)
            {
                return "Profile already exists."; 
            }

            var hashedPassword = _jWTHelper.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                Password = hashedPassword
            };

            var address = new Address
            {
                StreetAndNumber = dto.StreetAndNumber,
                PostalCode = dto.PostalCode,
                City = dto.City
            };

            var contact = new Contact
            {
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            await _userRepository.RegisterUserAsync(user, address, contact, dto.UserName);

            return "Profile registered successfully."; 
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var hashedPassword = _jWTHelper.HashPassword(password);

            var user = await _userRepository.GetProfileByEmailAndPasswordAsync(email, hashedPassword);
            if (user == null)
            {
                return "Invalid email or password.";
            }

            return _jWTHelper.GenerateJwtToken(user); 
        }
    }
}
