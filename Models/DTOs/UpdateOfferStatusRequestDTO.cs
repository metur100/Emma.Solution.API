using Emma.Api.Models.Enum;

namespace Emma.Api.Models.DTOs
{
    public class UpdateOfferStatusRequestDTO
    {
        public int OfferId { get; set; }
        public StatusType StatusType { get; set; }
    }
}
