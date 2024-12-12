using Emma.Api.Models;

namespace Emma.Api.Repositories.StatusRepository
{
    public interface IStatusRepository
    {
        Task CreateStatusAsync(Status status);
        Task<int> GetStatusTypeByOfferId(int offerId);
    }
}
