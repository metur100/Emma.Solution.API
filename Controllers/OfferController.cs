using Emma.Api.Handlers.OfferHandler;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfferDetails(int id)
        {
            var offer = await _offerHandler.GetOfferDetailsByIdAsync(id);
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

        [HttpPatch]
        public async Task<IActionResult> AddCounterOffer([FromBody] CounterOfferDTO request)
        {
            await _offerHandler.AddCounterOffer(request);
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

        [HttpGet("Transit")]
        public async Task<IActionResult> GetTransitOffersAsync()
        {
            var userId = _userClaimHelper.GetUserId();
            var offers = await _offerHandler.GetTransitOffersAsync(userId);
            return Ok(offers);
        }

        [HttpGet("Declined")]
        public async Task<IActionResult> GetDeclinedOffersAsync()
        {
            var userId = _userClaimHelper.GetUserId();
            var offers = await _offerHandler.GetDeclinedOffersAsync(userId);
            return Ok(offers);
        }

        [HttpPost("Decline/{offerId}")]
        public async Task<IActionResult> AddDeclineOffer(int offerId)
        {
            if (offerId <= 0)
                return BadRequest(new { Message = "Invalid offer ID." });

            try
            {
                await _offerHandler.AddDeclineOfferAsync(offerId);
                return Ok(new { Message = "Offer has been successfully declined." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while declining the offer.", Details = ex.Message });
            }
        }

        [HttpGet("Count")]
        public async Task<IActionResult> GetAllOffersCount()
        {
            try
            {
                var result = await _offerHandler.GetAllOffersCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/Transports/Count")]
        public async Task<IActionResult> GetAllTransportsCount()
        {
            try
            {
                var result = await _offerHandler.GetAllTransportsCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("/Delivered/Count")]
        public async Task<IActionResult> GetAllDeliveredOffersCount()
        {
            try
            {
                var result = await _offerHandler.GetAllDeliveredOffersCountAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
