using Emma.Api.Handlers.FreightHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class FreightController(IFreightHandler freightHandler) : ControllerBase
    {
        private readonly IFreightHandler _freightHandler = freightHandler;

        [HttpPost]
        public async Task<IActionResult> CreateFreightAsync([FromBody] Freight freight)
        {
            try
            {
                var profileId = freight.ProfileId;
                await _freightHandler.CreateFreightAsync(freight, profileId);
                return Ok("Freight created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFreightsAsync()
        {
            try
            {
                var result = await _freightHandler.GetAllFreightsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{profileId}/{freightId}")]
        public async Task<IActionResult> GetFreightInformations(int profileId, int freightId)
        {
            try
            {
                var result = await _freightHandler.GetFreightInformationByFreightIdAsync(profileId, freightId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetFreightsByUserIdAsync(int profileId)
        {
            try
            {
                var result = await _freightHandler.GetFreightsByProfileIdAsync(profileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
