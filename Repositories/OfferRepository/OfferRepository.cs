using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.OfferRepository
{
    public class OfferRepository(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Offer>> GetOffersForUserAsync(int profileId)
        {
            return await _context.Offers
                .Where(o => _context.Freights.Any(f => f.Id == o.FreightId && f.ProfileId == profileId) ||
                            _context.Vehicles.Any(v => v.Id == o.VehicleId && v.ProfileId == profileId))
                .ToListAsync();
        }

        public async Task<Offer?> GetOfferByIdAsync(int offerId)
        {
            return await _context.Offers
                .Include(o => o.Freight)
                .Include(o => o.Vehicle)
                .FirstOrDefaultAsync(o => o.Id == offerId);
        }

        public async Task CreateOfferAsync(Offer offer)
        {
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
        }
    }
}
