namespace Emma.Api.Models.DTOs
{
    public class RatingProfileDTO
    {
        public int OfferId { get; set; }
        public float RatingValue { get; set; }
        public required int OfferForUserProfileId { get; set; }
        public required int OfferFromUserProfileId { get; set; }
    }
}
