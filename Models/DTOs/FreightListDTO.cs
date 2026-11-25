namespace Emma.Api.Models.DTOs
{
    public class FreightListDTO
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string? ProfilePicture { get; set; }
        public required string ProfileName { get; set; }
        public required string LoadingAddress { get; set; }
        public required string UnloadingAddress { get; set; }
        public DateTime AvailabilityDate { get; set; }
        public string? TransportPrice { get; set; }
        public string? GoodsType { get; set; }
        public string? GoodsWeight { get; set; }
        public string? Distance { get; set; }
    }
}
