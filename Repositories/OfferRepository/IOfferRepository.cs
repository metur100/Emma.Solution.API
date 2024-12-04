using Emma.Api.Models;

namespace Emma.Api.Repositories.OfferRepository
{
    public interface IOfferRepository
    {
        Task<IEnumerable<Offer>> GetOffersForUserAsync(int userId);
        Task<Offer?> GetOfferByIdAsync(int offerId);
        Task CreateOfferAsync(Offer offer);
    }
}
