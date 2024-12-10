using Emma.Api.Models.Enum;
using Emma.Api.Models;
using Emma.Api.Repositories.StatusRepository;

namespace Emma.Api.Handlers.StatusHandler
{
    public class StatusHandler (IStatusRepository statusRepository) : IStatusHandler
    {
        private readonly IStatusRepository _statusRepository = statusRepository;

        public async Task UpdateOfferStatusAsync(int offerId, StatusType statusType)
        {
            var newStatus = new Status
            {
                OfferId = offerId,
                StatusType = (int)statusType,
                CreatedAt = DateTime.UtcNow
            };

            await _statusRepository.CreateStatusAsync(newStatus);
        }
    }
}
