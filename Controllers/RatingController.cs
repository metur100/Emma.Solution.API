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
                return Ok("Ocjena uspješno kreirana.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Došlo je do greške: {ex.Message}");
            }
        }
    }
}
