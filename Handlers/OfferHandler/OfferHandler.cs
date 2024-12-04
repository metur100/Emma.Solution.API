using Emma.Api.Models;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.OfferHandler
{
    public class OfferHandler
    {
        private readonly IOfferRepository _repository;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IFreightRepository _freightRepository;

        public OfferHandler(IOfferRepository repository, IVehicleRepository vehicleRepository, IFreightRepository freightRepository)
        {
            _repository = repository;
            _vehicleRepository = vehicleRepository;
            _freightRepository = freightRepository;
        }

        public async Task<IEnumerable<Offer>> GetOffersForUserAsync(int userId)
        {
            return await _repository.GetOffersForUserAsync(userId);
        }

        public async Task<Offer?> GetOfferDetailsAsync(int offerId)
        {
            return await _repository.GetOfferByIdAsync(offerId);
        }

        public async Task CreateOfferAsync(int freightId, int vehicleId, string newTransportPrice, string? comment, int profileId)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null || vehicle.ProfileId != profileId)
            {
                throw new UnauthorizedAccessException("Vehicle does not belong to the user.");
            }

            var freight = await _freightRepository.GetFreightByIdAsync(freightId);
            if (freight == null)
            {
                throw new ArgumentException("Freight not found.");
            }

            var offer = new Offer
            {
                FreightId = freightId,
                VehicleId = vehicleId,
                Status = "Pending",
                NewTransportPrice = newTransportPrice,
                Comment = comment
            };

            await _repository.CreateOfferAsync(offer);
        }
    }
}
