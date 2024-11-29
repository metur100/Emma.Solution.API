using Emma.Api.Handlers.UserHandler;
using Emma.Api.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
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

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _userHandler.LoginAsync(dto.Email, dto.Password);

            if (result == "Invalid email or password.")
            {
                return Unauthorized(new { message = result });
            }

            return Ok(new { Token = result });
        }

    }
}
