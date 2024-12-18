using Emma.Api.Handlers.ProfileHandler;
using Emma.Api.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProfileController(IProfileHandler profileHandler) : ControllerBase
    {
        private readonly IProfileHandler _profileHandler = profileHandler;

        [HttpPost]
        public async Task<IActionResult> CreateProfile([FromBody] CreateProfileDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = await _profileHandler.CreateProfileAsync(
                request.UserName,
                request.ProfilePicture,
                request.StreetAndNumber,
                request.PostalCode,
                request.City,
                request.Email,
                request.PhoneNumber,
                request.UserId
            );

            return Ok(new { Message = "User created successfully", UserId = userId });
        }

        [HttpGet("Names/{userId}")]
        public async Task<IActionResult> GetProfileNamesByUserIdAsync(int userId)
        {
            try
            {
                var userNames = await _profileHandler.GetProfileNamesByUserIdAsync(userId);
                return Ok(userNames);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving user names.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserProfileInformations(int id)
        {
            try
            {
                var userNames = await _profileHandler.GetUserProfileInformations(id);
                return Ok(userNames);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving user names.");
            }
        }

        [HttpGet("Name/{userId}")]
        public async Task<IActionResult> GetProfileNameByUserId(int userId)
        {
            try
            {
                var userName = await _profileHandler.GetProfileNameByUserId(userId);
                return Ok(userName);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving user names.");
            }
        }

        [HttpGet("/Profiles/{userId}")]
        public async Task<IActionResult> GetProfilesByUserIdAsync(int userId)
        {
            try
            {
                var profiles = await _profileHandler.GetProfilesByUserIdAsync(userId);
                return Ok(profiles);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving user names.");
            }
        }

        [HttpPut("{profileId}")]
        public async Task<IActionResult> UpdateProfile(int profileId, [FromBody] UpdateProfileDTO updateProfileDTO)
        {
            if (updateProfileDTO == null)
            {
                return BadRequest("Profile data is required.");
            }

            try
            {
                await _profileHandler.UpdateProfileAsync(profileId, updateProfileDTO);
                return Ok("Profile updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Management/{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            try
            {
                var profile = await _profileHandler.GetUpdateProfileByIdAsync(id);
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error fetching profile: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfileById(int id)
        {
            try
            {
                await _profileHandler.DeleteProfileByIdAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting profile: {ex.Message}");
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetAllProfilesCount()
        {
            try
            {
                var result = await _profileHandler.GetAllProfilesCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
