using Emma.Api.Handlers.VehicleHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VehicleController(IVehicleHandler vehicleHandler) : ControllerBase
    {
        private readonly IVehicleHandler _vehicleHandler = vehicleHandler;

        [HttpPost]
        public async Task<IActionResult> CreateVehicleAsync([FromBody] Vehicle vehicle)
        {
            try
            {
                var userId = vehicle.UserId;
                await _vehicleHandler.CreateVehicleAsync(vehicle, userId);
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

        [HttpGet("{userId}/{vehicleId}")]
        public async Task<IActionResult> GetVehicleInformation(int userId, int vehicleId)
        {
            try
            {
                var result = await _vehicleHandler.GetVehicleInformationByVehicleIdAsync(userId, vehicleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
