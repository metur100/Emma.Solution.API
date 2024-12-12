using Emma.Api.Handlers.StatusHandler;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class StatusController (IStatusHandler statusHandler) : ControllerBase
    {
        private readonly IStatusHandler _statusHandler = statusHandler;

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateOfferStatusRequestDTO request)
        {
            if (request == null || request.OfferId <= 0 || !Enum.IsDefined(typeof(StatusType), request.StatusType))
            {
                return BadRequest("Invalid request data.");
            }

            try
            {
                await _statusHandler.UpdateOfferStatusAsync(request.OfferId, request.StatusType);
                return Ok("Status updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception (use a logger in production)
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("{offerId}")]
        public async Task<IActionResult> GetStatusTypeByOfferId(int offerId)
        {
            var result = await _statusHandler.GetStatusTypeByOfferId(offerId);
            return Ok(result);
        }
    }
}
