using Emma.Api.Models;
using Emma.Api.Repositories.RatingRepository;
using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Repositories.ProfileRepository;

namespace Emma.Api.Handlers.RatingHandler
{
    public class RatingHandler (IRatingRepository ratingRepository,
        IJWTHelper jWTHelper,
        IProfileRepository profileRepository) : IRatingHandler
    {
        private readonly IRatingRepository _ratingRepository = ratingRepository;
        private readonly IJWTHelper _jwTHelper = jWTHelper;
        private readonly IProfileRepository _profileRepository = profileRepository;

        public async Task AddNewRating(int offerId, float ratingValue, int offerFromUserProfileId, int offerForUserProfileId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Nevažeći ID ponude.", nameof(offerId));

            if (ratingValue < 1 || ratingValue > 5)
                throw new ArgumentException("Ocjena mora biti između 1 i 5.", nameof(ratingValue));

            try
            {
                var userId = _jwTHelper.GetUserId();
                var loggedInProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);

                if (!loggedInProfileIds.Any())
                    throw new UnauthorizedAccessException("Nijedan profil nije povezan sa prijavljenim korisnikom.");

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
                    throw new UnauthorizedAccessException("Prijavljeni korisnik nije povezan sa ovom ponudom.");
                }

                var existingRating = await _ratingRepository.GetRatingForProfileAndOfferAsync(profileToRateId, offerId);

                if (existingRating != null)
                {
                    throw new InvalidOperationException("Već ste ocijenili ovaj profil za ovu ponudu.");
                }

                var profileToRate = await _profileRepository.GetProfileByIdAsync(profileToRateId)
                    ?? throw new InvalidOperationException("Profil koji želite ocijeniti nije pronađen.");
                var profileToRateName = profileToRate.Name;

                var newRating = new Rating
                {
                    ProfileId = profileToRateId,
                    RatingValue = ratingValue,
                    OfferId = offerId,
                    ProfileToRateName = profileToRateName,
                    CreatedAt = DateTime.UtcNow
                };

                await _ratingRepository.CreateRatingAsync(newRating);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Došlo je do greške prilikom kreiranja ocjene za ID ponude {offerId}.", ex);
            }
        }
    }
}
