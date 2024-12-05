using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.OfferRepository
{
    public interface IOfferRepository
    {
        Task<IEnumerable<Offer>> GetOffersAsync(int userId);
        Task<OfferDetailsDTO?> GetOfferByIdAsync(int offerId);
        Task CreateOfferAsync(Offer offer, int userId);
    }
}
