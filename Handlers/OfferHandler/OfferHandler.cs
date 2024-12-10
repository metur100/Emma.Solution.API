using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Models.Enum;
using Emma.Api.Repositories.OfferRepository;
using Emma.Api.Repositories.StatusRepository;

namespace Emma.Api.Handlers.OfferHandler
{
    public class OfferHandler(IOfferRepository repository, IStatusRepository statusRepository) : IOfferHandler
    {
        private readonly IOfferRepository _offerRepository = repository;
        private readonly IStatusRepository _statusRepository = statusRepository;

        public async Task<OfferDetailsDTO?> GetOfferDetailsAsync(int offerId)
        {
            return await _offerRepository.GetOfferByIdAsync(offerId);
        }

        public async Task CreateOfferAsync(Offer offer, int userId)
        {
            await _offerRepository.CreateOfferAsync(offer, userId);

            var initialStatus = new Status
            {
                OfferId = offer.Id,
                StatusType = (int)StatusType.Created,
                CreatedAt = DateTime.UtcNow
            };

            await _statusRepository.CreateStatusAsync(initialStatus);
        }

        public async Task<IEnumerable<Offer>> GetReceivedOffersAsync(int userId)
        {
           return await _offerRepository.GetReceivedOffersAsync(userId);
        }

        public async Task<IEnumerable<Offer>> GetSentOffersAsync(int userId)
        {
            return await _offerRepository.GetSentOffersAsync(userId);
        }
    }
}
