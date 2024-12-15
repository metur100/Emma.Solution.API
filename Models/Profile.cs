namespace Emma.Api.Models
{
    public class Profile
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required string Name { get; set; }
        public string? ProfilePicture { get; set; }
        public int AddressId { get; set; }
        public int ContactId { get; set; }
    }
}
