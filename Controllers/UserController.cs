using Emma.Api.Handlers.UserHandler;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserHandler _userHandler;

        public UserController(IUserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid user data.");
            }

            var result = await _userHandler.RegisterAsync(dto);

            if (result == "Profile already exists.")
            {
                return Conflict(new { message = result }); 
            }

            return Ok(new { message = result }); 
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
