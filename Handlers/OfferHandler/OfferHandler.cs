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

        public async Task<OfferDetailsDTO?> GetOfferByIdAsync(int offerId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.");

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
                OfferForStuffId = offer.OfferForStuffId,
                OfferFromUserProfileId = offer.OfferFromUserProfileId,
                OfferForUserProfileId = offer.OfferForUserProfileId,
                OfferFromStuffId = offer.OfferFromStuffId,
                TransporterId = offer.TransporterProfileId,
                FreightIdForTransport = offer.FreightIdForTransport,
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
            if (userId <= 0)
                throw new ArgumentException("The user ID must be a positive integer.", nameof(userId));

            try
            {
                var profileIds = await _profileRepository.GetUserProfileIdsByUserIdAsync(userId);
                var vehicleIds = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(profileIds);
                var freightIds = await _freightRepository.GetFreightIdsByProfileIdsAsync(profileIds);
                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();

                var validOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!validOfferIds.Any())
                    return Enumerable.Empty<Offer>(); 

                var receivedOffers = await _offerRepository.GetOffersByIdsAsync(validOfferIds);
                if (receivedOffers == null)
                    throw new InvalidOperationException("Failed to retrieve offers from the repository.");

                var filteredOffers = receivedOffers
                    .Where(o => !profileIds.Contains(o.OfferFromUserProfileId))
                    .Where(o => vehicleIds.Contains(o.VehicleId) || freightIds.Contains(o.FreightId))
                    .ToList();

                return filteredOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while fetching received offers.", ex);
            }
        }

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            if (offer == null)
                throw new ArgumentNullException(nameof(offer), "Offer cannot be null.");

            if (userId <= 0)
                throw new ArgumentException("User ID must be a positive integer.", nameof(userId));

            try
            {
                await _offerRepository.CreateOfferAsync(offer, userId);

                if (offer.Id <= 0)
                    throw new InvalidOperationException("Offer creation failed. The Offer ID is invalid.");
                var initialStatus = new Status
                {
                    OfferId = offer.Id,
                    StatusType = (int)StatusType.Created,
                    CreatedAt = DateTime.UtcNow
                };

                await _statusRepository.CreateStatusAsync(initialStatus);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while creating the offer.", ex);
            }
        }

        public async Task<IEnumerable<Offer>> GetSentOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<Offer>();

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<Offer>();

                var validOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Created || ls.LatestStatusType == (int)StatusType.CounterOffer)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!validOfferIds.Any())
                    return Enumerable.Empty<Offer>();

                var sentOffers = await _offerRepository.GetOffersByProfileIdsAsync(userProfileIds);
                if (sentOffers == null)
                    return Enumerable.Empty<Offer>();

                return sentOffers.Where(o => validOfferIds.Contains(o.Id)).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving sent offers.", ex);
            }
        }

        public async Task<IEnumerable<Offer>> GetTransitOffersAsync(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID.");

            var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
            var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfileIds);
            var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfileIds);

            var allOffers = await _offerRepository.GetOffersAsync();

            var transitOffers = new List<Offer>();
            foreach (var offer in allOffers)
            {
                bool isTransitOffer = false;

                if (userProfileIds.Contains(offer.OfferFromUserProfileId))
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

            try
            {
                var userProfileIds = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfileIds == null || !userProfileIds.Any())
                    return Enumerable.Empty<Offer>();

                var userProfiles = await _profileRepository.GetProfileIdsByUserIdAsync(userId);
                if (userProfiles == null || !userProfiles.Any())
                    return Enumerable.Empty<Offer>();

                var userVehicles = await _vehicleRepository.GetVehicleIdsByProfileIdsAsync(userProfiles);
                var userFreights = await _freightRepository.GetFreightIdsByProfileIdsAsync(userProfiles);

                var latestStatuses = await _statusRepository.GetLatestStatusesAsync();
                if (latestStatuses == null || !latestStatuses.Any())
                    return Enumerable.Empty<Offer>();

                var declinedOfferIds = latestStatuses
                    .Where(ls => ls.LatestStatusType == (int)StatusType.Declined)
                    .Select(ls => ls.OfferId)
                    .ToList();

                if (!declinedOfferIds.Any())
                    return Enumerable.Empty<Offer>();

                var allOffers = await _offerRepository.GetOffersByIdsAsync(declinedOfferIds);
                if (allOffers == null || !allOffers.Any())
                    return Enumerable.Empty<Offer>();

                var declinedOffers = allOffers
                    .Where(o =>
                        (userProfileIds.Contains(o.OfferFromUserProfileId) && declinedOfferIds.Contains(o.Id)) ||
                        (!userProfileIds.Contains(o.OfferFromUserProfileId) &&
                         (userVehicles.Contains(o.VehicleId) || userFreights.Contains(o.FreightId)) &&
                         declinedOfferIds.Contains(o.Id)))
                    .ToList();

                return declinedOffers;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while retrieving declined offers.", ex);
            }
        }
    }
}
