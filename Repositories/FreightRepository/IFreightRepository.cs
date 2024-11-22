using Emma.Api.Models;

namespace Emma.Api.Repositories.FreightRepository
{
    public interface IFreightRepository
    {
        Task CreateFreightAsync(Freight freight, int userId);
    }
}
