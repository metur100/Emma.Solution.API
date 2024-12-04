using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.OfferRepository
{
    public class OfferRepository(AppDbContext context) : IOfferRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Offer>> GetOffersForUserAsync(int profileId)
        {
            return await _context.Offers
                .Where(o => _context.Freights.Any(f => f.Id == o.FreightId && f.ProfileId == profileId) ||
                            _context.Vehicles.Any(v => v.Id == o.VehicleId && v.ProfileId == profileId))
                .ToListAsync();
        }

        public async Task<OfferDetailsDTO?> GetOfferByIdAsync(int offerId)
        {
            return await (from o in _context.Offers
                          join v in _context.Vehicles on o.VehicleId equals v.Id
                          join f in _context.Freights on o.FreightId equals f.Id
                          select new OfferDetailsDTO
                          {
                              Id = o.Id,
                              OfferFor = "v.UserId.ToString()", 
                              OfferFrom = "f.UserId.ToString()", 
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
            // Retrieve the profile IDs for the vehicle and freight
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

            // Retrieve the user IDs for the vehicle and freight owners
            var owners = await _context.Profiles
                .Where(p => p.Id == vehicleProfileId || p.Id == freightProfileId)
                .Select(p => new { p.Id, p.UserId })
                .ToListAsync();

            var vehicleOwner = owners.FirstOrDefault(o => o.Id == vehicleProfileId);
            var freightOwner = owners.FirstOrDefault(o => o.Id == freightProfileId);

            if (vehicleOwner == null || freightOwner == null)
                throw new Exception("Profiles for the vehicle or freight not found.");

            // Check ownership
            bool isVehicleOwnedByCurrentUser = vehicleOwner.UserId == userId;
            bool isFreightOwnedByCurrentUser = freightOwner.UserId == userId;

            // Return true if the same user owns both
            return isVehicleOwnedByCurrentUser && isFreightOwnedByCurrentUser;
        }
    }
}
