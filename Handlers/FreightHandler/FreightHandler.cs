using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.FreightRepository;

namespace Emma.Api.Handlers.FreightHandler
{
    public class FreightHandler(IFreightRepository freightRepository) : IFreightHandler
    {
        private readonly IFreightRepository _freightRepository = freightRepository;

        public async Task CreateFreightAsync(Freight freight, int userId)
        {
            try
            {
                await _freightRepository.CreateFreightAsync(freight, userId);
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

        public async Task<FreightInformationsDTO> GetFreightInformationByFreightIdAsync(int userId, int freigthId)
        {
            return await _freightRepository.GetFreightInformationByFreightIdAsync(userId, freigthId);
        }
    }
}
