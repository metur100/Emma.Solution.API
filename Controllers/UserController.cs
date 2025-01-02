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
                return BadRequest(new { message = "Email and password are required." });
            }

            var result = await _userHandler.LoginAsync(dto.Email, dto.Password, Response);

            if (result == "Invalid email or password.")
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }
            else if (result == "NotConfirmed")
            {
                return Unauthorized(new { message = "NotConfirmed" });
            }

            return Ok(new { token = result });
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

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] string email)
        {
            try
            {
                var result = await _userHandler.ForgotPasswordAsync(email);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var result = await _userHandler.ResetPasswordAsync(resetPasswordDto.Token, resetPasswordDto.NewPassword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = HttpContext.Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new { message = "Refresh token is missing." });
                }

                var newAccessToken = await _userHandler.RefreshTokenAsync(refreshToken, Response);

                if (newAccessToken == null)
                {
                    return Unauthorized(new { message = "Invalid or expired refresh token." });
                }

                return Ok(new { Token = newAccessToken });
            }
            catch (Exception)
            {
                return Unauthorized(new { message = "Token refresh failed." });
            }
        }
    }
}
