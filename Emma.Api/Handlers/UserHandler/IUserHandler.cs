namespace Emma.Api.Handlers.UserHandler
{
    public interface IUserHandler
    {
        Task<int> CreateUser(string name, string? profilePicture, string streetAndNumber, string postalCode, string city, string email, string phoneNumber);
    }
}
