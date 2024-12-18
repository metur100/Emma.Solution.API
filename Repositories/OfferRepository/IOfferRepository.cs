using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.OfferRepository
{
    public interface IOfferRepository
    {
        Task CreateOfferAsync(Offer offer, int userId);
        Task<List<Offer>> GetOffersByIdsAsync(IEnumerable<int> offerIds);
        Task<List<Offer>> GetOffersByProfileIdsAsync(IEnumerable<int> profileIds);
        Task<List<Offer>> GetOffersAsync();
        Task<Offer?> GetOfferByIdAsync(int offerId);
        Task UpdateOfferAsync(Offer offer);
        Task<List<Offer>> GetOffersByVehicleIdAsync(int vehicleId);
        Task DeleteOffersByVehicleIdAsync(int vehicleId);
        Task<List<Offer>> GetOffersByFreightIdAsync(int freightId);
        Task DeleteOffersByFreightIdAsync(int freightId);
        Task<int> GetOffersCountAsync();
    }
}
