using Emma.Api.Handlers.UserHandler;
using Emma.Api.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserHandler userHandler) : ControllerBase
    {
        private readonly IUserHandler _userHandler = userHandler;

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = await _userHandler.CreateUserAsync(
                request.Name,
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
        public async Task<IActionResult> GetUserNames()
        {
            try
            {
                var userNames = await _userHandler.GetUserNamesAsync();
                return Ok(userNames);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving user names.");
            }
        }
    }
}
