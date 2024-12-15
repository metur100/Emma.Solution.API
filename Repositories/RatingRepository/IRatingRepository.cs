using Emma.Api.Models;

namespace Emma.Api.Repositories.RatingRepository
{
    public interface IRatingRepository
    {
        Task CreateRatingAsync(Rating rating);
    }
}
