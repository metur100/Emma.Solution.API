using Emma.Api.Models;
using System.Threading.Tasks;

namespace Emma.Api.Repositories.RatingRepository
{
    public interface IRatingRepository
    {
        Task CreateRatingAsync(Rating rating);
        Task<Rating?> GetRatingForProfileAndOfferAsync(int profileToRateId, int offerId);
    }
}
