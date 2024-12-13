using Emma.Api.Helpers.JWTHelper;
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
        IFreightRepository freightRepository,
        IJWTHelper jWTHelper) : IOfferHandler
    {
        private readonly IOfferRepository _offerRepository = repository;
        private readonly IStatusRepository _statusRepository = statusRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IFreightRepository _freightRepository = freightRepository;
        private readonly IJWTHelper _jwTHelper = jWTHelper;

        public async Task<OfferDetailsDTO?> GetOfferByIdAsync(int offerId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.");

            var userId = _jwTHelper.GetUserId();

            var offer = await _offerRepository.GetOfferByIdAsync(offerId);
            if (offer == null)
                return null;

            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(offer.VehicleId);
            var freight = await _freightRepository.GetFreightByIdAsync(offer.FreightId);

            if (vehicle == null || freight == null)
                throw new InvalidOperationException("Offer data is incomplete.");

            return new OfferDetailsDTO
            {
                Id = offer.Id,
                OfferFor = offer.OfferFor,
                OfferFrom = offer.OfferFrom,
                Transporter = offer.Transporter,
                FreightForTransport = offer.FreightForTransport,
                FirstOfferPrice = freight.TransportPrice,
                SecondOfferPrice = offer.NewTransportPrice,
                Comment = offer.Comment,
                Brand = vehicle.Brand,
                PlateNumber = vehicle.PlateNumber,
                Description = vehicle.Description,
                BodyType = vehicle.BodyType,
                PayloadCapacity = vehicle.PayloadCapacity,
                CargoSpaceVolume = vehicle.CargoSpaceVolume,
                Length = vehicle.Length,
                Weight = vehicle.Weight,
                LoadingAddress = freight.LoadingAddress,
                UnloadingAddress = freight.UnloadingAddress,
                TransportPrice = freight.TransportPrice,
                Distance = freight.Distance,
                GoodsDescription = freight.GoodsDescription,
                GoodsType = freight.GoodsType,
                GoodsWeight = freight.GoodsWeight,
                GoodsLength = freight.GoodsLength,
                GoodsPrice = freight.GoodsPrice,
                AvailabilityDate = freight.AvailabilityDate
            };
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
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            var userProfileNames = await _profileRepository.GetProfileNamesByUserIdAsync(userId);
            if (!userProfileNames.Any())
                return Enumerable.Empty<Offer>();

            var latestStatuses = await _statusRepository.GetLatestStatusesAsync();

            var validOfferIds = latestStatuses
                .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                .Select(ls => ls.OfferId)
                .ToList();

            if (!validOfferIds.Any())
                return Enumerable.Empty<Offer>();
            var sentOffers = await _offerRepository.GetOffersByProfileNamesAsync(userProfileNames);

            return sentOffers.Where(o => validOfferIds.Contains(o.Id)).ToList();
        }

        public async Task<IEnumerable<Offer>> GetTransitOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            var userProfileNames = await _profileRepository.GetProfileNamesByUserIdAsync(userId);
            var userProfiles = await _profileRepository.GetProfileIdsByUserIdAsync(userId);

            var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfiles);
            var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfiles);

            var allOffers = await _offerRepository.GetOffersAsync();

            var transitOffers = new List<Offer>();
            foreach (var offer in allOffers)
            {
                bool isTransitOffer = false;

                if (userProfileNames.Contains(offer.OfferFrom))
                {
                    isTransitOffer = await _statusRepository.HasStatusGreaterOrEqualAsync(offer.Id, (int)StatusType.PreperingForTransit);
                }
                else if ((userVehicles.Contains(offer.VehicleId) || userFreights.Contains(offer.FreightId)) &&
                         await _statusRepository.HasStatusGreaterOrEqualAsync(offer.Id, (int)StatusType.PreperingForTransit))
                {
                    isTransitOffer = true;
                }

                if (isTransitOffer)
                {
                    transitOffers.Add(offer);
                }
            }

            return transitOffers;
        }

        public async Task<IEnumerable<Offer>> GetDeclinedOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            var userProfileNames = await _profileRepository.GetProfileNamesByUserIdAsync(userId);
            var userProfiles = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
            var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfiles);
            var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfiles);

            var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
            var declinedOfferIds = latestStatuses
                .Where(ls => ls.LatestStatusType == (int)StatusType.Declined)
                .Select(ls => ls.OfferId)
                .ToList();

            var allOffers = await _offerRepository.GetOffersByIdsAsync(declinedOfferIds);

            var declinedOffers = allOffers
                .Where(o =>
                    (userProfileNames.Contains(o.OfferFrom) && declinedOfferIds.Contains(o.Id)) ||
                    (!userProfileNames.Contains(o.OfferFrom) &&
                     (userVehicles.Contains(o.VehicleId) || userFreights.Contains(o.FreightId)) &&
                     declinedOfferIds.Contains(o.Id)))
                .ToList();

            return declinedOffers;
        }
    }
}
