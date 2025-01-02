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

        public async Task<string> LoginAsync(string email, string password, HttpResponse response)
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

                if (!user.Confirmed)
                {
                    return "NotConfirmed";
                }

                var accessToken = await _jWTHelper.GenerateJwtTokenAsync(user);
                var refreshToken = _jWTHelper.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
                await _userRepository.UpdateAsync(user);

                response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return accessToken;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during login.", ex);
            }
        }

        public async Task<string> RegisterUserWithConfirmationAsync(RegisterUserDto registerUserDto)
        {
            if (registerUserDto == null)
                throw new ArgumentNullException(nameof(registerUserDto));
            try
            {

                var hashedPassword = _jWTHelper.HashPassword(registerUserDto.Password);

                var user = new User
                {
                    Email = registerUserDto.Email,
                    Password = hashedPassword,
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
                    Name = registerUserDto.ProfileName,
                    AddressId = address.Id,
                    ContactId = contact.Id,
                    UserId = user.Id,
                };

                await _profileRepository.CreateProfileAsync(profile, user.Id);

                var confirmationUrl = $"https://emma-logistics-git-master-metur100s-projects.vercel.app/confirm/{user.ConfirmationToken}";
                var emailSubject = "Potvrdite vašu registraciju / Confirm your registration";
                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1abc9c;'>Dobrodošli, {registerUserDto.ProfileName}!</h2>
                    <p>Hvala vam što ste se registrovali na našoj platformi. Da biste dovršili proces registracije i aktivirali vaš nalog, molimo vas da potvrdite svoju registraciju klikom na sledeći link:</p>
                    <p>
                        <a href='{confirmationUrl}' style='background-color: #1abc9c; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Potvrdi registraciju</a>
                    </p>
                    <p>Ako imate bilo kakvih pitanja ili vam je potrebna pomoć, slobodno nas kontaktirajte.</p>
                    <p>Lijep pozdrav,<br>Vaš Emma Logistics tim</p>

                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>

                    <h2 style='color: #1abc9c;'><em>Welcome, {registerUserDto.ProfileName}!</em></h2>
                    <p><em>Thank you for registering on our platform. To complete your registration and activate your account, please confirm your registration by clicking the following link:</em></p>
                    <p>
                        <a href='{confirmationUrl}' style='background-color: #1abc9c; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'><em>Confirm Registration</em></a>
                    </p>
                    <p><em>If you have any questions or need assistance, feel free to contact us.</em></p>
                    <p><em>Best regards,<br>Your Emma Logistics Team</em></p>
                </div>";

                _emailServiceHelper.SendEmail(new List<string> { user.Email }, emailSubject, emailBody);

                return "User registered successfully. Please check your email to confirm your registration.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during user registration.", ex);
            }
        }

        public async Task<string> ConfirmUserAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token is invalid.", nameof(token));

            try
            {
                var user = await _userRepository.GetUserByTokenAsync(token);
                if (user == null)
                    return "Vaš link za potvrdu nije validan ili je istekao";

                if (user.Confirmed)
                    return "Vas račun je vec potvrđen";

                user.Confirmed = true;
                await _userRepository.UpdateAsync(user);

                return "Vaš profil je uspješno potvrđen.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during user confirmation.", ex);
            }
        }

        public async Task<string> ForgotPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.");

            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user == null)
                    return "User with this email does not exist.";

                var resetToken = Guid.NewGuid().ToString();
                user.ResetPasswordToken = resetToken;
                user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);

                await _userRepository.UpdateAsync(user);

                var resetUrl = $"https://emma-logistics-git-master-metur100s-projects.vercel.app/{resetToken}";
                var emailSubject = "Resetuj lozinku / Reset your password";
                var emailBody = $@"
                <div style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <h2 style='color: #1abc9c;'>Resetirajte Vašu Lozinku</h2>
                    <p>Primili smo zahtjev za resetovanje Vaše lozinke. Kliknite na link ispod kako biste resetovali lozinku:</p>
                    <p>
                        <a href='{resetUrl}' style='background-color: #1abc9c; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Resetuj Lozinku</a>
                    </p>
                    <p>Ovaj link će isteći za 1 sat. Ako niste zatražili resetovanje lozinke, zanemarite ovaj email.</p>
                    <p>Lijep pozdrav,<br>Vaš Emma Logistics tim</p>

                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #ddd;'>

                    <h2 style='color: #1abc9c;'><em>Reset Your Password</em></h2>
                    <p><em>We have received a request to reset your password. Click the link below to reset your password:</em></p>
                    <p>
                        <a href='{resetUrl}' style='background-color: #1abc9c; color: #fff; padding: 10px 20px; text-decoration: none; border-radius: 5px;'><em>Reset Password</em></a>
                    </p>
                    <p><em>This link will expire in 1 hour. If you did not request a password reset, please ignore this email.</em></p>
                    <p><em>Best regards,<br>Your Emma Logistics Team</em></p>
                </div>";

                _emailServiceHelper.SendEmail(new List<string> { user.Email }, emailSubject, emailBody);

                return "A password reset link has been sent to your email.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during forgot password.", ex);
            }
        }

        public async Task<string> ResetPasswordAsync(string token, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Invalid token.");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be null or empty.");

            try
            {
                var user = await _userRepository.GetUserByResetTokenAsync(token);
                if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                    return "Invalid or expired token.";

                user.Password = _jWTHelper.HashPassword(newPassword);
                user.ResetPasswordToken = null;
                user.ResetTokenExpiry = null;

                await _userRepository.UpdateAsync(user);

                return "Your password has been successfully reset.";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred during password reset.", ex);
            }
        }

        public async Task<string?> RefreshTokenAsync(string refreshToken, HttpResponse response)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return null;
            }

            var newAccessToken = await _jWTHelper.GenerateJwtTokenAsync(user);
            var newRefreshToken = _jWTHelper.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return newAccessToken;
        }
    }
}
