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
            else if (result == "NotConfirmed")
            {
                return Unauthorized(new { message = result });
            }

            return Ok(new { Token = result });
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userHandler.RegisterUserWithConfirmationAsync(registerUserDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet("Confirm/{token}")]
        public async Task<IActionResult> ConfirmUser(string token)
        {
            try
            {
                var result = await _userHandler.ConfirmUserAsync(token);
                if (result == "Vaš profil je uspješno potvrđen.")
                    return Ok(result);

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
