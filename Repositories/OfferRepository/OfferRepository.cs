using Emma.Api.Helpers.JWTHelper;
using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.OfferRepository
{
    public class OfferRepository(AppDbContext context, IJWTHelper jWTHelper) : IOfferRepository
    {
        private readonly AppDbContext _context = context;
        private readonly IJWTHelper _jWTHelper = jWTHelper;

        public async Task<IEnumerable<Offer>> GetOffersAsync(int userId)
        {
            // Step 1: Get all profiles for the user
            var userProfiles = await _context.Profiles
                .Where(p => p.UserId == userId)
                .Select(p => p.Id)
                .ToListAsync();

            // Step 2: Get all vehicles and freights linked to these profiles
            var userVehicles = await _context.Vehicles
                .Where(v => userProfiles.Contains(v.ProfileId))
                .Select(v => v.Id)
                .ToListAsync();

            var userFreights = await _context.Freights
                .Where(f => userProfiles.Contains(f.ProfileId))
                .Select(f => f.Id)
                .ToListAsync();

            // Step 3: Find received offers
            var receivedOffers = await _context.Offers
                .Where(o => userVehicles.Contains(o.VehicleId) || userFreights.Contains(o.FreightId))
                .ToListAsync();

            return receivedOffers;
        }


        public async Task<OfferDetailsDTO?> GetOfferByIdAsync(int offerId)
        {
            var userId = _jWTHelper.GetUserId();
            return await (from o in _context.Offers
                          join v in _context.Vehicles on o.VehicleId equals v.Id
                          join f in _context.Freights on o.FreightId equals f.Id
                          join p1 in _context.Profiles on v.ProfileId equals p1.Id
                          join p2 in _context.Profiles on f.ProfileId equals p2.Id
                          join u1 in _context.Users on p1.UserId equals u1.Id
                          join u2 in _context.Users on p2.UserId equals u2.Id
                          select new OfferDetailsDTO
                          {
                              Id = o.Id,
                              OfferFor = (userId == p1.UserId) ? (f.GoodsType ?? "Unknown Freight") : (v.Brand ?? "Unknown Vehicle"),
                              OfferFrom = (userId == p1.UserId) ? p1.Name : p2.Name,
                              Status = o.Status,
                              FirstOfferPrice = f.TransportPrice,
                              SecondOfferPrice = o.NewTransportPrice,
                              Comment = o.Comment,
                              Brand = v.Brand,
                              PlateNumber = v.PlateNumber,
                              Description = v.Description,
                              BodyType = v.BodyType,
                              PayloadCapacity = v.PayloadCapacity,
                              CargoSpaceVolume = v.CargoSpaceVolume,
                              Length = v.Length,
                              Weight = v.Weight,
                              LoadingAddress = f.LoadingAddress,
                              UnloadingAddress = f.UnloadingAddress,
                              TransportPrice = f.TransportPrice,
                              Distance = f.Distance,
                              GoodsDescription = f.GoodsDescription,
                              GoodsType = f.GoodsType,
                              GoodsWeight = f.GoodsWeight,
                              GoodsLength = f.GoodsLength,
                              GoodsPrice = f.GoodsPrice,
                              AvailabilityDate = f.AvailabilityDate
                          }).FirstOrDefaultAsync(o => o.Id == offerId);
        }

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            // Ensure the user is not creating an invalid offer
            if (await IsInvalidOfferAsync(userId, offer.VehicleId, offer.FreightId))
            {
                throw new InvalidOperationException("You cannot create an offer with both your vehicle and your freight.");
            }

            // Set the initial status and save the offer
            offer.Status = "Ponuda u obradi";
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
    }
}
