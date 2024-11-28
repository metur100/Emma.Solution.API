using Emma.Api.Handlers.ProfileHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly ProfileHandler _profileHandler;

        public ProfileController(ProfileHandler profileHandler)
        {
            _profileHandler = profileHandler;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User profile)
        {
            if (string.IsNullOrEmpty(profile.Email) || string.IsNullOrEmpty(profile.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _profileHandler.RegisterAsync(profile);

            if (result == "Profile registered successfully.")
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User profile)
        {
            if (string.IsNullOrEmpty(profile.Email) || string.IsNullOrEmpty(profile.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _profileHandler.LoginAsync(profile.Email, profile.Password);

            if (result == "Login successful.")
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }
    }
}
