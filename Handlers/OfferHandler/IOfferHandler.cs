using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.OfferHandler
{
    public interface IOfferHandler
    {
        Task<IEnumerable<Offer>> GetOffersForUserAsync(int userId);
        Task<OfferDetailsDTO?> GetOfferDetailsAsync(int offerId);
        Task CreateOfferAsync(Offer offer, int userId);
    }
}
