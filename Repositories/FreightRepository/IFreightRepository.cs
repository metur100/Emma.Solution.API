using Emma.Api.Models;
using Emma.Api.Models.DTOs;

namespace Emma.Api.Repositories.FreightRepository
{
    public interface IFreightRepository
    {
        Task CreateFreightAsync(Freight freight, int profileId);
        Task<List<FreightListDTO>> GetAllFreightsAsync();
        Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int profileId, int freightId);
        Task<IEnumerable<FreightManagementDTO>> GetFreightsByUserIdAsync(int userId);
        Task UpdateFreightAsync(Freight freight);
        Task<Freight> GetFreightByIdAsync(int id);
        Task DeleteFreightByIdAsync(int freightId);
        Task<string?> GetProfileNameByFreightIdAsync(int freightId);
    }
}
