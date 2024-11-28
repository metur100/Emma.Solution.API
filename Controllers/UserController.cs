using Emma.Api.Handlers.UserHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _userHandler.RegisterAsync(user);

            if (result == "Profile registered successfully.")
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] User user)
        {
            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _userHandler.LoginAsync(user.Email, user.Password);

            if (result == "Login successful.")
            {
                return Ok(result);
            }

            return Unauthorized(result);
        }
    }
}
