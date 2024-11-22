using Emma.Api.Models;

namespace Emma.Api.Repositories.AddressRepository
{
    public class AddressRepository(AppDbContext context) : IAddressRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<int> CreateAddress(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address.Id;
        }
    }
}
