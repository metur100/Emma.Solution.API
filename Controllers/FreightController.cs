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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFreightByIdAsync(int id)
        {
            try
            {
                var result = await _freightHandler.GetFreightByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/Freights/{userId}")]
        public async Task<IActionResult> GetFreightsByUser(int userId)
        {
            try
            {
                var result = await _freightHandler.GetFreightsByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFreightAsync(int id, [FromBody] Freight freight)
        {
            try
            {
                if (id != freight.Id)
                    return BadRequest("Freight ID mismatch.");

                await _freightHandler.UpdateFreightAsync(freight);
                return Ok("Freight updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFreightAsync(int id)
        {
            try
            {
                await _freightHandler.DeleteFreightByIdAsync(id);
                return Ok("Freight deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Profile/Id/{freightId}")]
        public async Task<IActionResult> GetProfileIdsByFreightIdAsync(int freightId)
        {
            try
            {
                var result = await _freightHandler.GetProfileIdsByFreightIdAsync(freightId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/ListFreights/{profileId}")]
        public async Task<IActionResult> GetFreightsByProfileIdAsync(int profileId)
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

        [HttpGet("Count")]
        public async Task<IActionResult> GetAllFreightsCount()
        {
            try
            {
                var result = await _freightHandler.GetAllFreightsCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Search/{searchTerm}")]
        public async Task<IActionResult> SearchFreights(string searchTerm)
        {
            try
            {
                var freights = await _freightHandler.HandleSearchFreightsAsync(searchTerm);
                return Ok(new { freights });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while searching freights: {ex.Message}" });
            }
        }

        [HttpGet("Search/{userId}/{searchTerm}")]
        public async Task<IActionResult> SearchFreightsByUserId(string searchTerm, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term cannot be empty or null." });
                }

                var freights = await _freightHandler.SearchFreightsByUserIdAsync(userId, searchTerm.Trim());

                if (freights == null || !freights.Any())
                {
                    return NotFound(new { message = "No freights found matching the search criteria." });
                }

                return Ok(new { freights });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while searching freights: {ex.Message}" });
            }
        }
    }
}
