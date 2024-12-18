namespace Emma.Api.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public required int VehicleId { get; set; }
        public required int FreightId { get; set; }
        public required int OfferForStuffId { get; set; }
        public required int OfferForUserProfileId { get; set; }
        public required int OfferFromStuffId { get; set; }
        public required int OfferFromUserProfileId { get; set; }
        public required int TransporterProfileId { get; set; }
        public required int FreightIdForTransport { get; set; }
        public string? NewTransportPrice { get; set; } 
        public string? Comment { get; set; }
        public string? CounterOfferPrice { get; set; }
        public int? CounterOfferProfileId { get; set; }
    }
}
