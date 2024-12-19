using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.AddressRepository;
using Emma.Api.Repositories.ContactRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.RatingRepository;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Handlers.ProfileHandler
{
    public class ProfileHandler(IProfileRepository userRepository,
        IAddressRepository addressRepository,
        IContactRepository contactRepository,
        IRatingRepository ratingRepository) : IProfileHandler
    {
        private readonly IProfileRepository _profileRepository = userRepository;
        private readonly IAddressRepository _addressRepository = addressRepository;
        private readonly IContactRepository _contactRepository = contactRepository;
        private readonly IRatingRepository _ratingRepository = ratingRepository;

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
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Profile name cannot be null or empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(streetAndNumber))
                throw new ArgumentException("Street and number cannot be null or empty.", nameof(streetAndNumber));

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new ArgumentException("Postal code cannot be null or empty.", nameof(postalCode));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be null or empty.", nameof(city));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.", nameof(userId));

            try
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

                var profile = new Profile
                {
                    Name = name,
                    ProfilePicture = profilePicture,
                    AddressId = addressId,
                    ContactId = contactId,
                    UserId = userId
                };

                return await _profileRepository.CreateProfileAsync(profile, userId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the profile.", ex);
            }
        }

        public async Task<List<ProfileDto>> GetProfileNamesByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("Invalid User ID.");

                var profiles = await _profileRepository.GetAllProfilesByUserIdAsync(userId);

                if (profiles == null || !profiles.Any())
                    throw new Exception($"No profiles found for user with ID {userId}");

                return profiles.Select(p => new ProfileDto
                {
                    Id = p.Id,
                    ProfileName = p.Name
                }).ToList();
            }
            catch (ArgumentException argEx)
            {
                throw new Exception($"Invalid input: {argEx.Message}", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching profiles for user with ID {userId}: {ex.Message}", ex);
            }
        }

        public async Task<ProfileInfoDTO> GetUserProfileInformations(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("Invalid User ID.");

                var profile = await _profileRepository.GetProfileByIdAsync(id)
                              ?? throw new Exception("User not found.");

                var address = await _addressRepository.GetAddressByIdAsync(profile.AddressId);
                var contact = await _contactRepository.GetContactByIdAsync(profile.ContactId);

                var (averageRating, ratingCount) = await _ratingRepository.GetRatingsSummaryForProfileAsync(profile.Id);

                var userProfile = new ProfileInfoDTO
                {
                    Id = profile.Id,
                    ProfileName = profile.Name,
                    ProfilePicture = profile.ProfilePicture,
                    StreetAndNumber = address?.StreetAndNumber ?? string.Empty,
                    PostalCode = address?.PostalCode ?? string.Empty,
                    City = address?.City ?? string.Empty,
                    Email = contact?.Email ?? string.Empty,
                    PhoneNumber = contact?.PhoneNumber ?? string.Empty,
                    AverageRating = averageRating,
                    RatingCount = ratingCount
                };

                return userProfile;
            }
            catch (ArgumentException argEx)
            {
                throw new Exception($"Invalid input: {argEx.Message}", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching user profile information for ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<string> GetProfileNameByUserId(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("Invalid User ID.");

                var profileName = await _profileRepository.GetProfileNameByUserIdAsync(userId);

                if (profileName == null)
                    throw new Exception($"Profile not found for UserId: {userId}");

                return profileName;
            }
            catch (ArgumentException argEx)
            {
                throw new Exception($"Invalid input: {argEx.Message}", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching profile name for UserId {userId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ProfileManagementDTO>> GetProfilesByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    throw new ArgumentException("Invalid User ID.");

                var profiles = await _profileRepository.GetProfilesByUserIdAsync(userId);

                if (!profiles.Any())
                    return Enumerable.Empty<ProfileManagementDTO>();

                var profileDtos = new List<ProfileManagementDTO>();

                foreach (var profile in profiles)
                {
                    var address = await _addressRepository.GetAddressByIdAsync(profile.AddressId);

                    profileDtos.Add(new ProfileManagementDTO
                    {
                        Id = profile.Id,
                        Name = profile.Name,
                        City = address?.City ?? "Unknown"
                    });
                }

                return profileDtos;
            }
            catch (ArgumentException argEx)
            {
                throw new Exception($"Invalid input: {argEx.Message}", argEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching profiles for UserId {userId}: {ex.Message}", ex);
            }
        }

        public async Task UpdateProfileAsync(int profileId, UpdateProfileDTO updateProfileDTO)
        {
            try
            {
                var profile = await _profileRepository.GetProfileByIdAsync(profileId) ?? throw new KeyNotFoundException("Profile not found.");
                profile.Name = updateProfileDTO.Name;

                var address = await _addressRepository.GetAddressByIdAsync(profile.AddressId);
                if (address != null)
                {
                    address.StreetAndNumber = updateProfileDTO.StreetAndNumber;
                    address.City = updateProfileDTO.City;
                    address.PostalCode = updateProfileDTO.PostalCode;

                    await _addressRepository.UpdateAddressAsync(address);
                }
                else
                {
                    throw new KeyNotFoundException("Address not found.");
                }

                var contact = await _contactRepository.GetContactByIdAsync(profile.ContactId);
                if (contact != null)
                {
                    contact.Email = updateProfileDTO.Email;
                    contact.PhoneNumber = updateProfileDTO.PhoneNumber;

                    await _contactRepository.UpdateContactAsync(contact);
                }
                else
                {
                    throw new KeyNotFoundException("Contact not found.");
                }

                await _profileRepository.UpdateProfileAsync(profile);
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception($"Not found: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating profile with ID {profileId}: {ex.Message}", ex);
            }
        }

        public async Task<UpdateProfileDTO> GetUpdateProfileByIdAsync(int id)
        {
            try
            {
                var profile = await _profileRepository.GetProfileByIdAsync(id);
                if (profile == null)
                {
                    throw new KeyNotFoundException("Profile not found");
                }

                var address = await _addressRepository.GetAddressByIdAsync(profile.AddressId);
                if (address == null)
                {
                    throw new KeyNotFoundException("Address not found");
                }

                var contact = await _contactRepository.GetContactByIdAsync(profile.ContactId);
                if (contact == null)
                {
                    throw new KeyNotFoundException("Contact not found");
                }

                var updateProfileDTO = new UpdateProfileDTO
                {
                    Id = profile.Id,
                    Name = profile.Name,
                    StreetAndNumber = address.StreetAndNumber,
                    City = address.City,
                    PostalCode = address.PostalCode,
                    Email = contact.Email,
                    PhoneNumber = contact.PhoneNumber
                };

                return updateProfileDTO;
            }
            catch (KeyNotFoundException ex)
            {
                throw new Exception($"Not found: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving profile information for ID {id}: {ex.Message}", ex);
            }
        }

        public async Task DeleteProfileByIdAsync(int profileId)
        {
            try
            {
                var profile = await _profileRepository.GetProfileByIdAsync(profileId) 
                    ?? throw new KeyNotFoundException($"Profile with ID {profileId} not found.");
                var userProfileCount = await _profileRepository.CountProfilesByUserIdAsync(profile.UserId);

                if (userProfileCount <= 1)
                {
                    throw new InvalidOperationException("Cannot delete the only profile for the user.");
                }

                await _profileRepository.DeleteProfileAsync(profileId);

                if (profile.AddressId > 0)
                {
                    await _addressRepository.DeleteAddressAsync(profile.AddressId);
                }

                if (profile.ContactId > 0)
                {
                    await _contactRepository.DeleteContactAsync(profile.ContactId);
                }
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception($"Operation failed: {ex.Message}", ex);
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting profile: {ex.Message}", ex);
            }
        }
        public async Task<int> GetAllProfilesCountAsync()
        {
            try
            {
                return await _profileRepository.GetProfilesCountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching all profiles: {ex.Message}");
            }
        }
    }
}
