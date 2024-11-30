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
        public async Task<IActionResult> GetProfilesByUserIdAsync(int userId)
        {
            try
            {
                var userNames = await _profileHandler.GetProfilesByUserIdAsync(userId);
                return Ok(userNames);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving user names.");
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfileInformations(int userId)
        {
            try
            {
                var userNames = await _profileHandler.GetUserProfileInformations(userId);
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
    }
}
