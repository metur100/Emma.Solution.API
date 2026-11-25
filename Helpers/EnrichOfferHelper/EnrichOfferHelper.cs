using Emma.Api.Models.DTOs;
using Emma.Api.Models;
using Emma.Api.Repositories.VehicleRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.ProfileRepository;
using Emma.Api.Repositories.StatusRepository;

namespace Emma.Api.Helpers.EnrichOfferHelper
{
    public class EnrichOfferHelper (IVehicleRepository vehicleRepository,
        IFreightRepository freightRepository,
        IProfileRepository profileRepository,
        IStatusRepository statusRepository) : IEnrichOfferHelper
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IFreightRepository _freightRepository = freightRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;
        private readonly IStatusRepository _statusRepository = statusRepository;

        /// <summary>
        /// An offer always links one vehicle and one freight. When the transporter sent the offer,
        /// the offer is "for" the freight and "from" the vehicle; otherwise it is the other way round.
        /// (Comparing ids of different tables is ambiguous, because a vehicle and a freight can share an id.)
        /// </summary>
        public static bool IsOfferFromTransporter(Offer offer) =>
            offer.OfferFromUserProfileId == offer.TransporterProfileId;

        public async Task<OfferListDTO> EnrichOfferAsync(Offer offer)
        {
            var vehicle = await _vehicleRepository.GetVehicleByIdAsync(offer.VehicleId);
            var freight = await _freightRepository.GetFreightByIdAsync(offer.FreightId);

            var vehicleName = vehicle != null ? $"{vehicle.Brand} - {vehicle.BodyType}" : "Unknown Vehicle";
            var freightName = freight != null ? $"{freight.GoodsType} - {freight.GoodsDescription}" : "Unknown Freight";

            var fromTransporter = IsOfferFromTransporter(offer);
            var offerForStuff = fromTransporter ? freightName : vehicleName;
            var offerFromStuff = fromTransporter ? vehicleName : freightName;

            var offerForUser = await _profileRepository.GetProfileNameByProfileIdAsync(offer.OfferForUserProfileId)
                                ?? "Unknown User";
            var offerFromUser = await _profileRepository.GetProfileNameByProfileIdAsync(offer.OfferFromUserProfileId)
                                 ?? "Unknown User";
            var transporter = await _profileRepository.GetProfileNameByProfileIdAsync(offer.TransporterProfileId)
                              ?? "Unknown Transporter";

            var freightForTransport = offer.FreightIdForTransport == offer.FreightId
                ? freight
                : await _freightRepository.GetFreightByIdAsync(offer.FreightIdForTransport);

            var latestStatus = await _statusRepository.GetLatestStatusByOfferIdAsync(offer.Id);

            return new OfferListDTO
            {
                Id = offer.Id,
                VehicleId = offer.VehicleId,
                FreightId = offer.FreightId,
                OfferForStuff = offerForStuff,
                OfferForUser = offerForUser,
                OfferFromStuff = offerFromStuff,
                OfferFromUser = offerFromUser,
                OfferForUserProfileId = offer.OfferForUserProfileId,
                OfferFromUserProfileId = offer.OfferFromUserProfileId,
                TransporterProfileId = offer.TransporterProfileId,
                Transporter = transporter,
                FreightForTransport = freightForTransport != null
                    ? $"{freightForTransport.GoodsType} - {freightForTransport.GoodsDescription}"
                    : "Unknown Freight",
                LoadingAddress = freight?.LoadingAddress,
                UnloadingAddress = freight?.UnloadingAddress,
                NewTransportPrice = offer.NewTransportPrice,
                TransportPrice = freightForTransport?.TransportPrice ?? "",
                CounterOfferPrice = offer.CounterOfferPrice,
                CounterOfferProfileId = offer.CounterOfferProfileId,
                Comment = offer.Comment,
                StatusType = latestStatus?.StatusType ?? 0,
                StatusUpdatedAt = latestStatus?.CreatedAt
            };
        }
    }
}
