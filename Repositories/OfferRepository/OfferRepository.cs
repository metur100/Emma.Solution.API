using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.OfferRepository
{
    public class OfferRepository(AppDbContext context) : IOfferRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            if (await IsInvalidOfferAsync(userId, offer.VehicleId, offer.FreightId))
            {
                throw new InvalidOperationException("You cannot create an offer with both your vehicle and your freight.");
            }

            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsInvalidOfferAsync(int userId, int vehicleId, int freightId)
        {
            var vehicleProfileId = await _context.Vehicles
                .Where(v => v.Id == vehicleId)
                .Select(v => v.ProfileId)
                .FirstOrDefaultAsync();

            if (vehicleProfileId == 0)
                throw new Exception("Vehicle not found.");

            var freightProfileId = await _context.Freights
                .Where(f => f.Id == freightId)
                .Select(f => f.ProfileId)
                .FirstOrDefaultAsync();

            if (freightProfileId == 0)
                throw new Exception("Freight not found.");

            var owners = await _context.Profiles
                .Where(p => p.Id == vehicleProfileId || p.Id == freightProfileId)
                .Select(p => new { p.Id, p.UserId })
                .ToListAsync();

            var vehicleOwner = owners.FirstOrDefault(o => o.Id == vehicleProfileId);
            var freightOwner = owners.FirstOrDefault(o => o.Id == freightProfileId);

            if (vehicleOwner == null || freightOwner == null)
                throw new Exception("Profiles for the vehicle or freight not found.");

            bool isVehicleOwnedByCurrentUser = vehicleOwner.UserId == userId;
            bool isFreightOwnedByCurrentUser = freightOwner.UserId == userId;

            return isVehicleOwnedByCurrentUser && isFreightOwnedByCurrentUser;
        }

        public async Task<List<Offer>> GetOffersByIdsAsync(IEnumerable<int> offerIds)
        {
            return await _context.Offers
                .Where(o => offerIds.Contains(o.Id))
                .ToListAsync();
        }

        public async Task<List<Offer>> GetOffersByProfileNamesAsync(IEnumerable<string> profileNames)
        {
            return await _context.Offers
                .Where(o => profileNames.Contains(o.OfferFromUser))
                .ToListAsync();
        }

        public async Task<List<Offer>> GetOffersAsync()
        {
            return await _context.Offers.ToListAsync();
        }

        public async Task<Offer?> GetOfferByIdAsync(int offerId)
        {
            return await _context.Offers
                .FirstOrDefaultAsync(o => o.Id == offerId);
        }
    }
}
