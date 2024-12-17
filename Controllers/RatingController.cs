using Emma.Api.Handlers.RatingHandler;
using Emma.Api.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Emma.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RatingController (IRatingHandler ratingHandler) : ControllerBase
    {
        private readonly IRatingHandler _ratingHandler = ratingHandler;

        [HttpPost]
        public async Task<IActionResult> UpdateStatus([FromBody] RatingProfileDTO request)
        {
            try
            {
                await _ratingHandler.AddNewRating(request.OfferId, request.RatingValue, 
                    request.OfferFromUserProfileId, request.OfferForUserProfileId);
                return Ok("Rating created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
