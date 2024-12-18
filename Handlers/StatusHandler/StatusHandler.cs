using Emma.Api.Models.Enum;
using Emma.Api.Models;
using Emma.Api.Repositories.StatusRepository;

namespace Emma.Api.Handlers.StatusHandler
{
    public class StatusHandler (IStatusRepository statusRepository) : IStatusHandler
    {
        private readonly IStatusRepository _statusRepository = statusRepository;

        public async Task AddOfferStatusAsync(int offerId, StatusType statusType)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.", nameof(offerId));

            try
            {
                var newStatus = new Status
                {
                    OfferId = offerId,
                    StatusType = (int)statusType,
                    CreatedAt = DateTime.UtcNow
                };

                await _statusRepository.CreateStatusAsync(newStatus);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while adding the status for offer ID {offerId}.", ex);
            }
        }

        public async Task<int> GetStatusTypeByOfferId(int offerId)
        {
            if (offerId <= 0)
                throw new ArgumentException("Invalid offer ID.", nameof(offerId));

            try
            {
                return await _statusRepository.GetStatusTypeByOfferId(offerId);
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"No status found for offer ID {offerId}.");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while retrieving the status type for offer ID {offerId}.", ex);
            }
        }
    }
}
