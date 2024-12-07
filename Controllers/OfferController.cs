using Emma.Api.Handlers.OfferHandler;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class OfferController(IOfferHandler offerHandler, IJWTHelper userClaimHelper) : ControllerBase
    {
        private readonly IOfferHandler _offerHandler = offerHandler;
        private readonly IJWTHelper _userClaimHelper = userClaimHelper;

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

            await _offerHandler.CreateOfferAsync(request, userId);

            return Ok();
        }

        [HttpGet("Received")]
        public async Task<IActionResult> GetReceivedOffersAsync()
        {
            var userId = _userClaimHelper.GetUserId();
            var offers = await _offerHandler.GetReceivedOffersAsync(userId);
            return Ok(offers);
        }

        [HttpGet("Sent")]
        public async Task<IActionResult> GetSentOffersAsync()
        {
            var userId = _userClaimHelper.GetUserId();
            var offers = await _offerHandler.GetSentOffersAsync(userId);
            return Ok(offers);
        }
    }
}
