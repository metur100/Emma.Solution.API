using Emma.Api.Handlers.VehicleHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleHandler _vehicleHandler;

        public VehicleController(IVehicleHandler vehicleHandler)
        {
            _vehicleHandler = vehicleHandler;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] Vehicle vehicle)
        {
            try
            {
                var userId= vehicle.Id;
                await _vehicleHandler.CreateVehicleAsync(vehicle, userId);
                return Ok("Vehicle created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); 
            }
        }
    }
}
