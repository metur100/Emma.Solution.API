namespace Emma.Api.Model
{
    public class User
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? ProfilePicture { get; set; }
        public int AddressId { get; set; }
        public int ContactId { get; set; }
        public float? Ratings { get; set; } 
    }
}
