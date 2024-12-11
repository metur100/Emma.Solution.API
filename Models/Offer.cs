namespace Emma.Api.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int FreightId { get; set; }
        public required string OfferFor { get; set; }
        public required string OfferFrom { get; set; }
        public required string Transporter { get; set; }
        public required string FreightForTransport { get; set; }
        public string? NewTransportPrice { get; set; } 
        public string? Comment { get; set; } 
    }
}
