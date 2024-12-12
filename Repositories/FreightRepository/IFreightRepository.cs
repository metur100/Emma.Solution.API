using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.FreightRepository
{
    public interface IFreightRepository
    {
        Task UpdateFreightAsync(Freight freight);
        Task<Freight?> GetFreightByIdAsync(int id);
        Task DeleteFreightAsync(Freight freight);
        Task<string?> GetProfileNameByFreightIdAsync(int freightId);
        Task AddFreightAsync(Freight freight);
        Task<List<Freight>> GetAllFreightsAsync();
        Task<Freight?> GetFreightByProfileAndIdAsync(int profileId, int freightId);
        Task<List<Freight>> GetFreightsByProfileIdsAsync(List<int> profileIds);
    }
}
