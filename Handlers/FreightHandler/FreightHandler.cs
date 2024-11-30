using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.FreightRepository;

namespace Emma.Api.Handlers.FreightHandler
{
    public class FreightHandler(IFreightRepository freightRepository) : IFreightHandler
    {
        private readonly IFreightRepository _freightRepository = freightRepository;

        public async Task CreateFreightAsync(Freight freight, int profileId)
        {
            try
            {
                await _freightRepository.CreateFreightAsync(freight, profileId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in vehicle handler: {ex.Message}");
            }
        }

        public async Task<List<FreightListDTO>> GetAllFreightsAsync()
        {
            var allFreights = await _freightRepository.GetAllFreightsAsync();
            return allFreights ?? throw new Exception("");
        }

        public async Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int profileId, int freigthId)
        {
            return await _freightRepository.GetFreightInformationByFreightIdAsync(profileId, freigthId);
        }

        public async Task<List<Freight>> GetFreightsByProfileIdAsync(int profileId)
        {
            return await _freightRepository.GetFreightsByProfileIdAsync(profileId);
        }
    }
}
