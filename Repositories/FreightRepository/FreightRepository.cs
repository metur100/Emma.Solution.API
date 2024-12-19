using Emma.Api.Models;
using Emma.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.FreightRepository
{
    public class FreightRepository(AppDbContext context) : IFreightRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddFreightAsync(Freight freight)
        {
            _context.Freights.Add(freight);
            await _context.SaveChangesAsync();
        }

        public async Task<Freight?> GetFreightByProfileAndIdAsync(int profileId, int freightId)
        {
            return await _context.Freights
                .Where(f => f.ProfileId == profileId && f.Id == freightId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Freight>> GetAllFreightsAsync()
        {
            return await _context.Freights.ToListAsync();
        }

        public async Task<int> GetFreightsCountAsync()
        {
            return await _context.Freights.CountAsync();
        }

        public async Task<List<Freight>> GetFreightsByProfileIdsAsync(List<int> profileIds)
        {
            return await _context.Set<Freight>()
                .Where(f => profileIds.Contains(f.ProfileId))
                .ToListAsync();
        }

        public async Task UpdateFreightAsync(Freight freight)
        {
            _context.Set<Freight>().Update(freight);
            await _context.SaveChangesAsync();
        }

        public async Task<Freight?> GetFreightByIdAsync(int id)
        {
            return await _context.Freights
                .Where(freight => freight.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteFreightAsync(Freight freight)
        {
            _context.Freights.Remove(freight);
            await _context.SaveChangesAsync();
        }

        public async Task<int?> GetProfileIdsByFreightIdAsync(int freightId)
        {
            return await (from vehicle in _context.Freights
                          join profile in _context.Profiles on vehicle.ProfileId equals profile.Id
                          where vehicle.Id == freightId
                          select profile.Id)
                      .FirstOrDefaultAsync();
        }

        public async Task<List<int>> GetFreightIdsByProfileIdsAsync(IEnumerable<int> profileIds)
        {
            return await _context.Freights
                .Where(f => profileIds.Contains(f.ProfileId))
                .Select(f => f.Id)
                .ToListAsync();
        }

        public async Task<List<Freight>> GetFreightsByProfileIdAsync(int profileId)
        {
            return await _context.Set<Freight>()
                .Where(p => p.ProfileId == profileId)
                .ToListAsync();
        }

        public async Task<List<FreightSearchDTO>> SearchFreightsAsync(string searchTerm)
        {
            var freightsWithProfileName = await (from f in _context.Freights
                                                 join p in _context.Profiles on f.ProfileId equals p.Id
                                                 select new
                                                 {
                                                     Freight = f,
                                                     ProfileName = p.Name
                                                 }).ToListAsync();

            var filteredFreights = freightsWithProfileName
                .Where(x =>
                    (x.ProfileName != null && x.ProfileName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) || // Filter by ProfileName
                    (x.Freight.LoadingAddress != null && x.Freight.LoadingAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.UnloadingAddress != null && x.Freight.UnloadingAddress.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.TransportPrice != null && x.Freight.TransportPrice.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.Status != null && x.Freight.Status.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.Distance != null && x.Freight.Distance.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.GoodsDescription != null && x.Freight.GoodsDescription.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.GoodsType != null && x.Freight.GoodsType.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.GoodsWeight != null && x.Freight.GoodsWeight.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.GoodsLength != null && x.Freight.GoodsLength.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (x.Freight.GoodsPrice != null && x.Freight.GoodsPrice.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .Select(x => new FreightSearchDTO
                {
                    Id = x.Freight.Id,
                    LoadingAddress = x.Freight.LoadingAddress,
                    UnloadingAddress = x.Freight.UnloadingAddress,
                    TransportPrice = x.Freight.TransportPrice,
                    Status = x.Freight.Status,
                    Distance = x.Freight.Distance,
                    GoodsDescription = x.Freight.GoodsDescription,
                    GoodsType = x.Freight.GoodsType,
                    GoodsWeight = x.Freight.GoodsWeight,
                    GoodsLength = x.Freight.GoodsLength,
                    GoodsPrice = x.Freight.GoodsPrice,
                    AvailabilityDate = x.Freight.AvailabilityDate,
                    ProfileId = x.Freight.ProfileId,
                    ProfileName = x.ProfileName
                })
                .ToList();

            return filteredFreights;
        }

    }
}
