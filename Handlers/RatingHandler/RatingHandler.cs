using Emma.Api.Models;
using Emma.Api.Repositories.RatingRepository;
using Emma.Api.Repositories.OfferRepository;

namespace Emma.Api.Handlers.RatingHandler
{
    public class RatingHandler (IRatingRepository ratingRepository,
        IOfferRepository offerRepository) : IRatingHandler
    {
        private readonly IRatingRepository _ratingRepository = ratingRepository;
        private readonly IOfferRepository _offerRepository = offerRepository;

        public async Task AddNewRating(int offerId, float ratingValue, int profileId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.", nameof(offerId));

            if (ratingValue < 1 || ratingValue > 5)
                throw new ArgumentException("Rating value must be between 1 and 5.", nameof(ratingValue));

            try
            {
                var offerDetails = await _offerRepository.GetOfferByIdAsync(offerId); 
                if (offerDetails == null)
                    throw new InvalidOperationException("Offer not found.");

                int userBeingRatedId = offerDetails.OfferFromUserProfileId == profileId ? offerDetails.OfferForUserProfileId : offerDetails.OfferFromUserProfileId;

                var newRating = new Rating
                {
                    ProfileId = userBeingRatedId,
                    RatingValue = ratingValue,
                    CreatedAt = DateTime.UtcNow
                };

                await _ratingRepository.CreateRatingAsync(newRating);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while creating the rating for offer ID {offerId}.", ex);
            }
        }

    }
}
