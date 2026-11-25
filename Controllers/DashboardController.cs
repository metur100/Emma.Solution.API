using Emma.Api.Handlers.FreightHandler;
using Emma.Api.Handlers.OfferHandler;
using Emma.Api.Handlers.VehicleHandler;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Emma.Api.Models.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class DashboardController(
        AppDbContext context,
        IJWTHelper jwtHelper,
        IOfferHandler offerHandler,
        IFreightHandler freightHandler,
        IVehicleHandler vehicleHandler) : ControllerBase
    {
        /// <summary>Everything the home screen needs for the logged-in user in a single request.</summary>
        [HttpGet("Summary")]
        public async Task<IActionResult> GetSummary()
        {
            var userId = jwtHelper.GetUserId();
            var profileIds = await context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            var myVehicles = await context.Vehicles.Where(v => profileIds.Contains(v.ProfileId)).ToListAsync();
            var myFreightCount = await context.Freights.CountAsync(f => profileIds.Contains(f.ProfileId));

            var received = (await offerHandler.GetReceivedOffersAsync(userId)).ToList();
            var sent = (await offerHandler.GetSentOffersAsync(userId)).ToList();
            var transports = (await offerHandler.GetTransitOffersAsync(userId)).ToList();

            var openFreights = (await freightHandler.GetAllFreightsAsync()).Count;
            var activeVehicles = (await vehicleHandler.GetAllVehiclesAsync()).Count;

            return Ok(new
            {
                profiles = profileIds.Count,
                myVehicles = myVehicles.Count,
                myActiveVehicles = myVehicles.Count(v => v.IsActive),
                myFreights = myFreightCount,
                receivedOffers = received.Count,
                sentOffers = sent.Count,
                activeTransports = transports.Count(t => t.StatusType == (int)StatusType.PreperingForTransit || t.StatusType == (int)StatusType.InTransit),
                completedTransports = transports.Count(t => t.StatusType == (int)StatusType.Arrived),
                openFreights,
                activeVehicles,
                latestReceived = received.OrderByDescending(o => o.StatusUpdatedAt).Take(3),
                latestTransports = transports
                    .Where(t => t.StatusType != (int)StatusType.Arrived)
                    .OrderByDescending(o => o.StatusUpdatedAt)
                    .Take(3)
            });
        }
    }
}
