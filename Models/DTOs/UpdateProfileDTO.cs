namespace Emma.Api.Models.DTOs
{
    public class UpdateProfileDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string StreetAndNumber { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public IFormFile? profilePicture {  get; set; }
    }

}
