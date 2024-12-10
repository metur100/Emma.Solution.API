using Emma.Api.Models;

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
    }
}

