using Emma.Api.Models.Enum;
using Emma.Api.Models;
using Emma.Api.Repositories.RatingRepository;

namespace Emma.Api.Handlers.RatingHandler
{
    public class RatingHandler (IRatingRepository ratingRepository) : IRatingHandler
    {
        private readonly IRatingRepository _ratingRepository = ratingRepository;

        public async Task AddNewRating(int profileId, float ratingValue)
        {
            if (profileId <= 0)
                throw new ArgumentException("Invalid profile ID.", nameof(profileId));

            try
            {
                var newRating = new Rating
                {
                    ProfileId = profileId,
                    RatingValue = ratingValue,
                    CreatedAt = DateTime.UtcNow
                };

                await _ratingRepository.CreateRatingAsync(newRating);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while creating the rating for profile ID {profileId}.", ex);
            }
        }
    }
}
