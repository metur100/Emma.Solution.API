using Emma.Api.Models;
using Emma.Api.Repositories.RatingRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Repositories.ProfileRepository;

namespace Emma.Api.Handlers.RatingHandler
{
    public class RatingHandler (IRatingRepository ratingRepository,
        IOfferRepository offerRepository,
        IJWTHelper jWTHelper,
        IProfileRepository profileRepository) : IRatingHandler
    {
        private readonly IRatingRepository _ratingRepository = ratingRepository;
        private readonly IOfferRepository _offerRepository = offerRepository;
        private readonly IJWTHelper _jwTHelper = jWTHelper;
        private readonly IProfileRepository _profileRepository = profileRepository;

        public async Task AddNewRating(int offerId, float ratingValue, int offerFromUserProfileId, int offerForUserProfileId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.", nameof(offerId));

            if (ratingValue < 1 || ratingValue > 5)
                throw new ArgumentException("Rating value must be between 1 and 5.", nameof(ratingValue));

            try
            {
                var userId = _jwTHelper.GetUserId();
                var loggedInProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);

                if (!loggedInProfileIds.Any())
                    throw new UnauthorizedAccessException("No profiles associated with the logged-in user.");

                int profileToRateId;

                if (loggedInProfileIds.Contains(offerFromUserProfileId))
                {
                    profileToRateId = offerForUserProfileId; 
                }
                else if (loggedInProfileIds.Contains(offerForUserProfileId))
                {
                    profileToRateId = offerFromUserProfileId; 
                }
                else
                {
                    throw new UnauthorizedAccessException("Logged-in user is not associated with this offer.");
                }

                var newRating = new Rating
                {
                    ProfileId = profileToRateId,
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
