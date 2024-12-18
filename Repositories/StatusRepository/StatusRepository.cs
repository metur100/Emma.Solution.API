using Emma.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Emma.Api.Repositories.StatusRepository
{
    public class StatusRepository(AppDbContext context) : IStatusRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateStatusAsync(Status status)
        {
            _context.Statuses.Add(status);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetStatusTypeByOfferId(int offerId)
        {
            var statusType = await _context.Statuses
                .Where(s => s.OfferId == offerId)
                .OrderByDescending(s => s.CreatedAt) 
                .Select(s => s.StatusType)
                .FirstOrDefaultAsync();

            return statusType;
        }

        public async Task<List<(int OfferId, int LatestStatusType)>> GetLatestStatusesAsync()
        {
            var statuses = await _context.Statuses
                .GroupBy(s => s.OfferId)
                .Select(g => new
                {
                    OfferId = g.Key,
                    LatestStatusType = g.OrderByDescending(s => s.CreatedAt).First().StatusType
                })
                .ToListAsync();

            return statuses
                .Select(x => (x.OfferId, x.LatestStatusType))
                .ToList();
        }

        public async Task<bool> HasStatusGreaterOrEqualAsync(int offerId, int statusType)
        {
            return await _context.Statuses
                .AnyAsync(s => s.OfferId == offerId && s.StatusType >= statusType);
        }

        public async Task GetStatusesByOfferIdsAsync(List<int> offerIds)
        {
            var statuses = await _context.Statuses
                .Where(s => offerIds.Contains(s.OfferId))
                .ToListAsync();
        }
    }
}

