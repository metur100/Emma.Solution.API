using Emma.Api.Handlers.OfferHandler;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OfferController(IOfferHandler offerHandler, IJWTHelper userClaimHelper) : ControllerBase
    {
        private readonly IOfferHandler _offerHandler = offerHandler;
        private readonly IJWTHelper _userClaimHelper = userClaimHelper;

        [HttpGet]
        public async Task<IActionResult> GetOffers()
        {
            var userId = _userClaimHelper.GetUserId(); 
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
            var userId = _userClaimHelper.GetUserId(); 

            try
            {
                await _offerHandler.CreateOfferAsync(
                    request.FreightId,
                    request.VehicleId,
                    request.NewTransportPrice!,
                    request.Comment,
                    userId
                );

                return CreatedAtAction(nameof(GetOfferDetails), new { id = request.Id }, null);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
