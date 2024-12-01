using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.FreightRepository
{
    public interface IFreightRepository
    {
        Task CreateFreightAsync(Freight freight, int profileId);
        Task<List<FreightListDTO>> GetAllFreightsAsync();
        Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int profileId, int freightId);
        Task<Freight> GetFreightByProfileIdAsync(int profileId);
        Task<IEnumerable<FreightSummaryDto>> GetFreightsByUserIdAsync(int userId);
        Task UpdateFreightAsync(Freight freight);
        Task<Freight> GetFreightByIdAsync(int id);
    }
}
