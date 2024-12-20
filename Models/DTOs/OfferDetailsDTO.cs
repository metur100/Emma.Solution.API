namespace Emma.Api.Models.DTOs
{
    public class OfferDetailsDTO
    {
        public int Id { get; set; }
        public required string OfferForStuff { get; set; }
        public required string OfferFromUser { get; set; }
        public required string OfferForUser { get; set; }
        public required int OfferForUserProfileId { get; set; }
        public required int OfferFromUserProfileId { get; set; }
        public required string OfferFromStuff { get; set; }
        public required string Transporter { get; set; }
        public required string FreightForTransport { get; set; }
        public required string ProfileToRateName { get; set; }
        public string? NewTransportPrice { get; set; }
        public string? CounterOfferTransportPrice { get; set; }
        public string? Comment { get; set; }
        public string? Brand { get; set; }
        public string? PlateNumber { get; set; }
        public string? Description { get; set; }
        public required string BodyType { get; set; }
        public string? PayloadCapacity { get; set; }
        public string? CargoSpaceVolume { get; set; }
        public string? Length { get; set; }
        public string? Weight { get; set; }
        public required string LoadingAddress { get; set; }
        public required string UnloadingAddress { get; set; }
        public required string TransportPrice { get; set; }
        public string? Distance { get; set; }
        public string? GoodsDescription { get; set; }
        public string? GoodsType { get; set; }
        public string? GoodsWeight { get; set; }
        public string? GoodsLength { get; set; }
        public string? GoodsPrice { get; set; }
        public DateTime AvailabilityDate { get; set; }
    }
}
