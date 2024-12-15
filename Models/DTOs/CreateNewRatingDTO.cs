namespace Emma.Api.Models.DTOs
{
    public class CreateNewRatingDTO
    {
        public required int ProfileId { get; set; }
        public required float RatingValue { get; set; }
    }
}
