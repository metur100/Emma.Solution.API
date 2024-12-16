namespace Emma.Api.Models
{
    public class Offer
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int FreightId { get; set; }
        public required int OfferForStuffId { get; set; }
        public required int OfferForUserProfileId { get; set; }
        public required int OfferFromStuffId { get; set; }
        public required int OfferFromUserProfileId { get; set; }
        public required int TransporterProfileId { get; set; }
        public required int FreightIdForTransport { get; set; }
        public string? NewTransportPrice { get; set; } 
        public string? Comment { get; set; } 
    }
}
