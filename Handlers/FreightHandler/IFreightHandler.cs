using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Handlers.FreightHandler
{
    public interface IFreightHandler
    {
        Task CreateFreightAsync(Freight freight, int profileId);
        Task<List<FreightListDTO>> GetAllFreightsAsync();
        Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int profileId, int freigthId);
        Task<Freight> GetFreightByProfileIdAsync(int profileId);
        Task<IEnumerable<FreightSummaryDto>> GetFreightsByUserAsync(int profileId);
        Task UpdateFreightAsync(Freight freight);
        Task DeleteFreightAsync(int profileId, int freightId);
    }
}
