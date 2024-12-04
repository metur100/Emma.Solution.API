using Emma.Api.Models;

namespace Emma.Api.Handlers.OfferHandler
{
    public interface IOfferHandler
    {
        Task<IEnumerable<Offer>> GetOffersForUserAsync(int userId);
        Task<Offer?> GetOfferDetailsAsync(int offerId);
        Task CreateOfferAsync(int freightId, int vehicleId, string newTransportPrice, string? comment, int userId);
    }
}
