using Emma.Api.Models;
using Emma.Api.Models.DTOs;
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


        public async Task CreateOfferAsync(Offer offer)
        {
            // Ensure the initial status is set
            offer.Status = "Ponuda u obradi";
            await _context.Offers.AddAsync(offer);
            await _context.SaveChangesAsync();
        }

    }
}
