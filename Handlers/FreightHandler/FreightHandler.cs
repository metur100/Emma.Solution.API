using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Emma.Api.Repositories.FreightRepository;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Freight> GetFreightByIdAsync(int id)
        {
            return await _freightRepository.GetFreightByIdAsync(id);
        }

        public async Task<IEnumerable<FreightManagementDTO>> GetFreightsByUserAsync(int userId)
        {
            return await _freightRepository.GetFreightsByUserIdAsync(userId);
        }

        public async Task UpdateFreightAsync(Freight freight)
        {
            var existingFreight = await _freightRepository.GetFreightByIdAsync(freight.Id);
            if (existingFreight == null)
                throw new KeyNotFoundException($"Freight with ID {freight.Id} not found.");

            existingFreight.LoadingAddress = freight.LoadingAddress;
            existingFreight.UnloadingAddress = freight.UnloadingAddress;
            existingFreight.TransportPrice = freight.TransportPrice;
            existingFreight.Status = freight.Status;
            existingFreight.Distance = freight.Distance;
            existingFreight.GoodsDescription = freight.GoodsDescription;
            existingFreight.GoodsType = freight.GoodsType;
            existingFreight.GoodsWeight = freight.GoodsWeight;
            existingFreight.GoodsLength = freight.GoodsLength;
            existingFreight.GoodsPrice = freight.GoodsPrice;
            existingFreight.AvailabilityDate = freight.AvailabilityDate;

            await _freightRepository.UpdateFreightAsync(existingFreight);
        }

        public async Task DeleteFreightByIdAsync(int id)
        {
            await _freightRepository.DeleteFreightByIdAsync(id);
        }

        public async Task<string?> GetProfileNameByFreightIdAsync(int freightId)
        {
            return await _freightRepository.GetProfileNameByFreightIdAsync(freightId);
        }
    }
}
