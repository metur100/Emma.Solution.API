using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.OfferRepository;

namespace Emma.Api.Handlers.OfferHandler
{
    public class OfferHandler(IOfferRepository repository) : IOfferHandler
    {
        private readonly IOfferRepository _offerRepository = repository;

        public async Task<IEnumerable<Offer>> GetOffersForUserAsync(int userId)
        {
            return await _offerRepository.GetOffersAsync(userId);
        }

        public async Task<OfferDetailsDTO?> GetOfferDetailsAsync(int offerId)
        {
            return await _offerRepository.GetOfferByIdAsync(offerId);
        }

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            await _offerRepository.CreateOfferAsync(offer, userId);
        }
    }
}
