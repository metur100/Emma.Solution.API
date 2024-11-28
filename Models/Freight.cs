namespace Emma.Api.Models
{
    public class Freight
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public required string LoadingAddress { get; set; }
        public required string UnloadingAddress { get; set; }
        public required string TransportPrice { get; set; }
        public string? Status { get; set; } 
        public string? Distance { get; set; } 
        public string? GoodsDescription { get; set; } 
        public string? GoodsType { get; set; } 
        public string? GoodsWeight { get; set; } 
        public string? GoodsLength { get; set; } 
        public string? GoodsPrice { get; set; } 
        public DateTime AvailabilityDate { get; set; }
    }
}
