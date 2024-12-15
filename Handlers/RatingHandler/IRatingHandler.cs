namespace Emma.Api.Handlers.RatingHandler
{
    public interface IRatingHandler
    {
        Task AddNewRating(int profileId, float ratingValue);
    }
}
