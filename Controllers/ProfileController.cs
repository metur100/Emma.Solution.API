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
        public async Task<IActionResult> CreateUser([FromBody] RegisterUserDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = await _profileHandler.CreateUserAsync(
                request.UserName,
                request.ProfilePicture,
                request.StreetAndNumber,
                request.PostalCode,
                request.City,
                request.Email,
                request.PhoneNumber
            );

            return Ok(new { Message = "User created successfully", UserId = userId });
        }

        [HttpGet("Names")]
        public async Task<IActionResult> GetProfileNames()
        {
            try
            {
                var userNames = await _profileHandler.GetUserNamesAsync();
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
    }
}
