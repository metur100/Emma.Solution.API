namespace Emma.Api.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public required int ProfileId { get; set; }
        public required int OfferId { get; set; }
        public required double RatingValue {  get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
