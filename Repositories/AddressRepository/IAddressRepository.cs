using Emma.Api.Models;

namespace Emma.Api.Repositories.AddressRepository
{
    public interface IAddressRepository
    {
        Task<int> CreateAddress(Address address);
        Task<Address?> GetAddressByIdAsync(int addressId);
        Task UpdateAddressAsync(Address address);
        Task DeleteAddressAsync(int addressId);
    }
}
