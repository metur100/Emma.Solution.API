using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.FreightRepository
{
    public interface IFreightRepository
    {
        Task<IEnumerable<FreightManagementDTO>> GetFreightsByUserIdAsync(int userId);
        Task UpdateFreightAsync(Freight freight);
        Task<Freight> GetFreightByIdAsync(int id);
        Task DeleteFreightByIdAsync(int freightId);
        Task<string?> GetProfileNameByFreightIdAsync(int freightId);
        Task AddFreightAsync(Freight freight);
        Task<List<Freight>> GetAllFreightsAsync();
        Task<Freight?> GetFreightByProfileAndIdAsync(int profileId, int freightId);
    }
}
