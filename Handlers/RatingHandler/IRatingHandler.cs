namespace Emma.Api.Handlers.RatingHandler
{
    public interface IRatingHandler
    {
        Task AddNewRating(int offerId, float ratingValue, int profileId);
    }
}
