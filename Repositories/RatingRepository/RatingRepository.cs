using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.RatingRepository
{
    public class RatingRepository (AppDbContext appDbContext) : IRatingRepository
    {
        private readonly AppDbContext _context = appDbContext;
        public async Task CreateRatingAsync(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<Rating?> GetRatingForProfileAndOfferAsync(int profileToRateId, int offerId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.ProfileId == profileToRateId && r.OfferId == offerId);
        }
    }
}
