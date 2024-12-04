using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.OfferHandler
{
    public class OfferHandler(IOfferRepository repository, 
        IVehicleRepository vehicleRepository, 
        IFreightRepository freightRepository) : IOfferHandler
    {
        private readonly IOfferRepository _offerRepository = repository;
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IFreightRepository _freightRepository = freightRepository;

        public async Task<IEnumerable<Offer>> GetOffersForUserAsync(int userId)
        {
            return await _offerRepository.GetOffersForUserAsync(userId);
        }

        public async Task<OfferDetailsDTO?> GetOfferDetailsAsync(int offerId)
        {
            return await _offerRepository.GetOfferByIdAsync(offerId);
        }

        public async Task CreateOfferAsync(int freightId, int vehicleId, string newTransportPrice, string? comment, int userId)
        {

            var offer = new Offer
            {
                FreightId = freightId,
                VehicleId = vehicleId,
                Status = "Pending",
                NewTransportPrice = newTransportPrice,
                Comment = comment
            };

            await _offerRepository.CreateOfferAsync(offer, userId);
        }
    }
}
