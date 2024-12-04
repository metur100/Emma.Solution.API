namespace Emma.Api.Models.DTOs
{
    public class CreateOfferDTO
    {
        public int VehicleId { get; set; }
        public int FreightId { get; set; }
        public required string Status { get; set; }
        public string? NewTransportPrice { get; set; }
        public string? Comment { get; set; }
    }
}
