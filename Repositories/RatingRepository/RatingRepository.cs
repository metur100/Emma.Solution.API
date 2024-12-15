using Emma.Api.Models;

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
    }
}
