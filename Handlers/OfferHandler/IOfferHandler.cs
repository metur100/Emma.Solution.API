using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.OfferHandler
{
    public interface IOfferHandler
    {
        Task<OfferDetailsDTO?> GetOfferDetailsByIdAsync(int offerId);
        Task CreateOfferAsync(Offer offer, int userId);
        Task<IEnumerable<OfferListDTO>> GetReceivedOffersAsync(int userId);
        Task<IEnumerable<OfferListDTO>> GetSentOffersAsync(int userId);
        Task<IEnumerable<OfferListDTO>> GetTransitOffersAsync(int userId);
        Task<IEnumerable<OfferListDTO>> GetDeclinedOffersAsync(int userId);
        Task AddCounterOffer(CounterOfferDTO counterOfferDTO);
        Task AddDeclineOfferAsync(int offerId);
        Task<int> GetAllOffersCountAsync();
        Task<int> GetAllTransportsCountAsync();
        Task<int> GetAllDeliveredOffersCountAsync();
        Task<IEnumerable<OfferListDTO>> SearchReceivedOffersAsync(int userId, string searchTerm);
        Task<IEnumerable<OfferListDTO>> SearchSentOffersAsync(int userId, string searchTerm);
        Task<IEnumerable<OfferListDTO>> SearchDeclinedOffersAsync(int userId, string searchTerm);
        Task<IEnumerable<OfferListDTO>> SearchTransitOffersAsync(int userId, string searchTerm);
    }
}
