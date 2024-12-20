using Emma.Api.Models.DTOs;
using Emma.Api.Models;
using Emma.Api.Repositories.VehicleRepository;
using Emma.Api.Repositories.FreightRepository;
using Emma.Api.Repositories.ProfileRepository;

namespace Emma.Api.Helpers.EnrichOfferHelper
{
    public class EnrichOfferHelper (IVehicleRepository vehicleRepository,
        IFreightRepository freightRepository,
        IProfileRepository profileRepository) : IEnrichOfferHelper
    {
        private readonly IVehicleRepository _vehicleRepository = vehicleRepository;
        private readonly IFreightRepository _freightRepository = freightRepository;
        private readonly IProfileRepository _profileRepository = profileRepository;

        public async Task<OfferListDTO> EnrichOfferAsync(Offer offer)
        {
            string offerForStuff = string.Empty;
            if (offer.VehicleId == offer.OfferForStuffId)
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(offer.OfferForStuffId);
                offerForStuff = vehicle != null
                    ? $"{vehicle.Brand} - {vehicle.BodyType}"
                    : "Unknown Vehicle";
            }
            else if (offer.FreightId == offer.OfferForStuffId)
            {
                var freight = await _freightRepository.GetFreightByIdAsync(offer.OfferForStuffId);
                offerForStuff = freight != null
                    ? $"{freight.GoodsType} - {freight.GoodsDescription}"
                    : "Unknown Freight";
            }

            // Resolve OfferFromStuff
            string offerFromStuff = string.Empty;
            if (offer.VehicleId == offer.OfferFromStuffId)
            {
                var vehicle = await _vehicleRepository.GetVehicleByIdAsync(offer.OfferFromStuffId);
                offerFromStuff = vehicle != null
                    ? $"{vehicle.Brand} - {vehicle.BodyType}"
                    : "Unknown Vehicle";
            }
            else if (offer.FreightId == offer.OfferFromStuffId)
            {
                var freight = await _freightRepository.GetFreightByIdAsync(offer.OfferFromStuffId);
                offerFromStuff = freight != null
                    ? $"{freight.GoodsType} - {freight.GoodsDescription}"
                    : "Unknown Freight";
            }

            var offerForUser = await _profileRepository.GetProfileNameByProfileIdAsync(offer.OfferForUserProfileId)
                                ?? "Unknown User";
            var offerFromUser = await _profileRepository.GetProfileNameByProfileIdAsync(offer.OfferFromUserProfileId)
                                 ?? "Unknown User";

            var transporter = await _profileRepository.GetProfileNameByProfileIdAsync(offer.TransporterProfileId)
                              ?? "Unknown Transporter";

            var freightForTransport = await _freightRepository.GetFreightByIdAsync(offer.FreightIdForTransport);

            var freightForTransportName = freightForTransport != null
                ? $"{freightForTransport.GoodsType} - {freightForTransport.GoodsDescription}"
                : "Unknown Freight";

            return new OfferListDTO
            {
                Id = offer.Id,
                VehicleId = offer.VehicleId,
                FreightId = offer.FreightId,
                OfferForStuff = offerForStuff,
                OfferForUser = offerForUser,
                OfferFromStuff = offerFromStuff,
                OfferFromUser = offerFromUser,
                Transporter = transporter,
                FreightForTransport = freightForTransportName,
                NewTransportPrice = offer.NewTransportPrice,
                TransportPrice = freightForTransport?.TransportPrice ?? "",
                CounterOfferPrice = offer.CounterOfferPrice,
                Comment = offer.Comment
            };
        }
    }
}
