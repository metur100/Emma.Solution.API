using Emma.Api.Models.DTOs;
using Emma.Api.Models;

namespace Emma.Api.Helpers.EnrichOfferHelper
{
    public interface IEnrichOfferHelper
    {
        Task<OfferListDTO> EnrichOfferAsync(Offer offer);
    }
}
