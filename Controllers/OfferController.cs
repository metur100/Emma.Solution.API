using Emma.Api.Handlers.ContactHandler;
using Emma.Api.Handlers.OfferHandler;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OfferController(IOfferHandler offerHandler) : ControllerBase
    {
        private readonly IOfferHandler _offerHandler = offerHandler;

        [HttpGet]
        public async Task<IActionResult> GetOffers()
        {
            var userId = GetUserId(); // Implement user ID retrieval from JWT or session
            var offers = await _offerHandler.GetOffersForUserAsync(userId);
            return Ok(offers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferDetails(int id)
        {
            var offer = await _offerHandler.GetOfferDetailsAsync(id);
            if (offer == null)
                return NotFound();

            return Ok(offer);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOffer([FromBody] Offer request)
        {
            var userId = GetUserId(); // Implement user ID retrieval from JWT or session
            try
            {
                await _offerHandler.CreateOfferAsync(request.FreightId, request.VehicleId, request.NewTransportPrice, request.Comment, userId);
                return CreatedAtAction(nameof(GetOfferDetails), new { id = request.FreightId }, null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int GetUserId()
        {
            // Extract user ID from JWT or session
            return int.Parse(User.Claims.First(c => c.Type == "id").Value);
        }
    }
}
