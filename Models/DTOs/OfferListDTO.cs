namespace Emma.Api.Models.DTOs
{
    public class OfferListDTO
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int FreightId { get; set; }
        public required string OfferForStuff { get; set; }
        public required string OfferForUser { get; set; }
        public required string OfferFromStuff { get; set; }
        public required string OfferFromUser { get; set; }
        public required string Transporter { get; set; }
        public required string FreightForTransport { get; set; }
        public required string TransportPrice { get; set; }
        public string? NewTransportPrice { get; set; }
        public string? Comment { get; set; }
        public string? CounterOfferPrice { get; set; }
        public int? CounterOfferProfileId { get; set; }
    }
}
