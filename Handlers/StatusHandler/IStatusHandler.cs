using Emma.Api.Models.Enum;

namespace Emma.Api.Handlers.StatusHandler
{
    public interface IStatusHandler
    {
        Task AddOfferStatusAsync(int offerId, StatusType statusType);
        Task<int> GetStatusTypeByOfferId(int offerId);
    }
}
