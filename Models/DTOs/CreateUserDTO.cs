namespace Emma.Api.Models.DTOs
{
    public class CreateUserDTO
    {
        public required string UserName { get; set; }
        public string? ProfilePicture { get; set; }
        public required string StreetAndNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
