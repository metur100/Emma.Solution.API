using Emma.Api.Models;
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
    }
}
