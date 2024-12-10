using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.OfferHandler
{
    public interface IOfferHandler
    {
        Task<OfferDetailsDTO?> GetOfferDetailsAsync(int offerId);
        Task CreateOfferAsync(Offer offer, int userId);
        Task<IEnumerable<Offer>> GetReceivedOffersAsync(int userId);
        Task<IEnumerable<Offer>> GetSentOffersAsync(int userId);
        Task<IEnumerable<Offer>> GetTransitOffersAsync(int userId);
    }
}
