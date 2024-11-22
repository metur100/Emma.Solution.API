using Emma.Api.Handlers.FreightHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FreightController(IFreightHandler freightHandler) : ControllerBase
    {
        private readonly IFreightHandler _freightHandler = freightHandler;

        [HttpPost]
        public async Task<IActionResult> CreateFreightAsync([FromBody] Freight freight)
        {
            try
            {
                var userId = freight.UserId;
                await _freightHandler.CreateFreightAsync(freight, userId);
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

        [HttpGet("{userId}/{freightId}")]
        public async Task<IActionResult> GetFreightInformations(int userId, int freightId)
        {
            try
            {
                var result = await _freightHandler.GetFreightInformationByFreightIdAsync(userId, freightId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
