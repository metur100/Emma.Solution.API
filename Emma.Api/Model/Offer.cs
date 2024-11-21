namespace Emma.Api.Model
{
    public class Offer
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int FreightId { get; set; }
        public required string Status { get; set; }
        public string? NewTransportPrice { get; set; } 
        public string? Comment { get; set; } 
    }
}
