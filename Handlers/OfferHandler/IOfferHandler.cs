using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.OfferHandler
{
    public interface IOfferHandler
    {
        Task<OfferDetailsDTO?> GetOfferByIdAsync(int offerId);
        Task CreateOfferAsync(Offer offer, int userId);
        Task<IEnumerable<OfferListDTO>> GetReceivedOffersAsync(int userId);
        Task<IEnumerable<OfferListDTO>> GetSentOffersAsync(int userId);
        Task<IEnumerable<OfferListDTO>> GetTransitOffersAsync(int userId);
        Task<IEnumerable<OfferListDTO>> GetDeclinedOffersAsync(int userId);
    }
}
