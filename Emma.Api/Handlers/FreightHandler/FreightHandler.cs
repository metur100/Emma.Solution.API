using Emma.Api.Models;
using Emma.Api.Repositories.FreightRepository;

namespace Emma.Api.Handlers.FreightHandler
{
    public class FreightHandler : IFreightHandler
    {
        private readonly IFreightRepository _freightRepository;

        public FreightHandler(IFreightRepository freightRepository)
        {
            _freightRepository = freightRepository;
        }

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
    }
}
