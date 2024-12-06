namespace Emma.Api.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int FreightId { get; set; }
        public string? OfferFor { get; set; }
        public string? OfferFrom { get; set; }
        public required string Status { get; set; }
        public string? NewTransportPrice { get; set; } 
        public string? Comment { get; set; } 
    }
}
