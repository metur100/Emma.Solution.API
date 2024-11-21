using Emma.Api.Models;

namespace Emma.Api.Handlers.FreightHandler
{
    public interface IFreightHandler
    {
        Task CreateFreightAsync(Freight freight, int userId);
    }
}
