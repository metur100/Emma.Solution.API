using Emma.Api.Models;
using Emma.Api.Models.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

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

        public async Task<List<Offer>> GetOffersByProfileIdsAsync(IEnumerable<int> profileIds)
        {
            return await _context.Offers
                .Where(o => profileIds.Contains(o.OfferFromUserProfileId))
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

        public async Task UpdateOfferAsync(Offer offer)
        {
            _context.Offers.Update(offer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Offer>> GetOffersByVehicleIdAsync(int vehicleId)
        {
            return await _context.Offers
                .Where(o => o.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task DeleteOffersByVehicleIdAsync(int vehicleId)
        {

            var offers = await _context.Offers
                .Where(o => o.VehicleId == vehicleId)
                .ToListAsync();

            if (offers.Any())
            {
                _context.Offers.RemoveRange(offers);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Offer>> GetOffersByFreightIdAsync(int freightId)
        {
            return await _context.Offers
                .Where(o => o.FreightId == freightId)
                .ToListAsync();
        }

        public async Task DeleteOffersByFreightIdAsync(int freightId)
        {

            var offers = await _context.Offers
                .Where(o => o.FreightId == freightId)
                .ToListAsync();

            if (offers.Any())
            {
                _context.Offers.RemoveRange(offers);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetOffersCountAsync()
        {
            return await _context.Offers.CountAsync();
        }

        public async Task<int> GetTransportsCountAsync(StatusType statusThreshold)
        {
            return await _context.Offers
                                 .Join(_context.Statuses,
                                       offer => offer.Id,
                                       status => status.OfferId,
                                       (offer, status) => new { offer, status })
                                 .Where(os => (int)os.status.StatusType >= (int)statusThreshold)
                                 .CountAsync();
        }

        public async Task<int> GetDeliveredCountAsync(StatusType statusThreshold)
        {
            return await _context.Offers
                                 .Join(_context.Statuses,
                                       offer => offer.Id,
                                       status => status.OfferId,
                                       (offer, status) => new { offer, status })
                                 .Where(os => (int)os.status.StatusType == (int)statusThreshold)
                                 .CountAsync();
        }
    }
}
