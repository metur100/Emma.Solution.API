using Emma.Api.Models;

namespace Emma.Api.Repositories.StatusRepository
{
    public interface IStatusRepository
    {
        Task CreateStatusAsync(Status status);
        Task<int> GetStatusTypeByOfferId(int offerId);
        Task<Status?> GetLatestStatusByOfferIdAsync(int offerId);
        Task<List<(int OfferId, int LatestStatusType)>> GetLatestStatusesAsync();
        Task<bool> HasStatusGreaterOrEqualAsync(int offerId, int statusType);
        Task GetStatusesByOfferIdsAsync(List<int> offerIds);
        Task DeleteStatusesByOfferIdsAsync(List<int> offerIds);
    }
}
