using Emma.Api.Models.Enum;

namespace Emma.Api.Handlers.StatusHandler
{
    public interface IStatusHandler
    {
        Task UpdateOfferStatusAsync(int offerId, StatusType statusType);
    }
}
