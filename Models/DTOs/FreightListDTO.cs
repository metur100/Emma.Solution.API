namespace Emma.Api.Models.DTOs
{
    public class FreightListDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? ProfilePicture { get; set; }
        public required string UserName { get; set; }
        public required string LoadingAddress { get; set; }
        public DateTime AvailabilityDate { get; set; }
    }
}
