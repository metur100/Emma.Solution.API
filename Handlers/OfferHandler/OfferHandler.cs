using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Models.Enum;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.StatusRepository;
using Emma.Api.Repositories.VehicleRepository;

namespace Emma.Api.Handlers.OfferHandler
{
    public class OfferHandler(IOfferRepository repository, 
        IStatusRepository statusRepository,
        IProfileRepository profileRepository,
        IVehicleRepository vehicleRepository,
        IFreightRepository freightRepository) : IOfferHandler
    {
        private readonly IOfferRepository _offerRepository = repository;
        private readonly IStatusRepository _statusRepository = statusRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IFreightRepository _freightRepository = freightRepository;

        public async Task<OfferDetailsDTO?> GetOfferDetailsAsync(int offerId)
        {
            return await _offerRepository.GetOfferByIdAsync(offerId);
        }

        public async Task<IEnumerable<Offer>> GetReceivedOffersAsync(int userId)
        {
            var profileNames = await _profileRepository.GetProfileNamesByUserIdAsync(userId);
            var profileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);
            var vehicleIds = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(profileIds);
            var freightIds = await _freightRepository.GetFreightIdsByProfileIdsAsync(profileIds);
            var latestStatuses = await _statusRepository.GetLatestStatusesAsync();

            var validOfferIds = latestStatuses
                .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                .Select(ls => ls.OfferId)
                .ToList();

            var receivedOffers = await _offerRepository.GetOffersByIdsAsync(validOfferIds);

            var filteredOffers = receivedOffers
                .Where(o => !profileNames.Contains(o.OfferFrom))
                .Where(o => vehicleIds.Contains(o.VehicleId) || freightIds.Contains(o.FreightId))
                .ToList();

            return filteredOffers;
        }

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            await _offerRepository.CreateOfferAsync(offer, userId);

            var initialStatus = new Status
            {
                OfferId = offer.Id,
                StatusType = (int)StatusType.Created,
                CreatedAt = DateTime.UtcNow
            };

            await _statusRepository.CreateStatusAsync(initialStatus);
        }

        public async Task<IEnumerable<Offer>> GetSentOffersAsync(int userId)
        {
            return await _offerRepository.GetSentOffersAsync(userId);
        }

        public async Task<IEnumerable<Offer>> GetTransitOffersAsync(int userId)
        {
            return await _offerRepository.GetTransitOffersAsync(userId);
        }

        public async Task<IEnumerable<Offer>> GetDeclinedOffersAsync(int userId)
        {
            return await _offerRepository.GetDeclinedOffersAsync(userId);
        }
    }
}
