using Emma.Api.Handlers.VehicleHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class VehicleController(IVehicleHandler vehicleHandler) : ControllerBase
    {
        private readonly IVehicleHandler _vehicleHandler = vehicleHandler;

        [HttpPost]
        public async Task<IActionResult> CreateVehicleAsync([FromBody] Vehicle vehicle)
        {
            try
            {
                var profileId = vehicle.ProfileId;
                await _vehicleHandler.CreateVehicleAsync(vehicle, profileId);
                return Ok("Vehicle created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVehiclesAsync()
        {
            try
            {
                var result = await _vehicleHandler.GetAllVehiclesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{profileId}/{vehicleId}")]
        public async Task<IActionResult> GetVehicleInformation(int profileId, int vehicleId)
        {
            try
            {
                var result = await _vehicleHandler.GetVehicleInformationByVehicleIdAsync(profileId, vehicleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleByIdAsync(int id)
        {
            try
            {
                var result = await _vehicleHandler.GetVehicleByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/Vehicles/{userId}")]
        public async Task<IActionResult> GetVehiclesByUserIdAsync(int userId)
        {
            try
            {
                var result = await _vehicleHandler.GetVehiclesByUserIdAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicleAsync(int id, [FromBody] Vehicle vehicle)
        {
            try
            {
                if (id != vehicle.Id)
                    return BadRequest("Vehicle ID mismatch.");

                await _vehicleHandler.UpdateVehicleAsync(vehicle);
                return Ok("Vehicle updated successfully.");
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
        public async Task<IActionResult> DeleteVehicleByIdAsync(int id)
        {
            try
            {
                await _vehicleHandler.DeleteVehicleByIdAsync(id);
                return Ok("Vehicle deleted successfully.");
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

        [HttpGet("Profile/Id/{vehicleId}")]
        public async Task<IActionResult> GetProfileIdByVehicleIdAsync(int vehicleId)
        {
            try
            {
                var result = await _vehicleHandler.GetProfileIdByVehicleIdAsync(vehicleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/ListVehicles/{profileId}")]
        public async Task<IActionResult> GetVehiclesByProfileIdAsync(int profileId)
        {
            try
            {
                var result = await _vehicleHandler.GetVehiclesByProfileIdAsync(profileId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetAllVehiclesCount()
        {
            try
            {
                var result = await _vehicleHandler.GetAllVehiclesCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("BodyTypes")]
        public async Task<IActionResult> GetAllBodyTypes()
        {
            try
            {
                var result = await _vehicleHandler.GetAllBodyTypesCountsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Search/{searchTerm}")]
        public async Task<IActionResult> SearchVehicles(string searchTerm)
        {
            try
            {
                var vehicles = await _vehicleHandler.HandleSearchVehiclesAsync(searchTerm);
                return Ok(new { vehicles });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while searching vehicles: {ex.Message}" });
            }
        }

        [HttpGet("Search/{userId}/{searchTerm}")]
        public async Task<IActionResult> SearchVehiclesByUserId(string searchTerm, int userId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(new { message = "Search term cannot be empty or null." });
                }

                var vehicles = await _vehicleHandler.SearchVehiclesByUserIdAsync(userId, searchTerm.Trim());

                if (vehicles == null || !vehicles.Any())
                {
                    return NotFound(new { message = "No vehicles found matching the search criteria." });
                }

                return Ok(new { vehicles });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred while searching vehicles: {ex.Message}" });
            }
        }
    }
}
