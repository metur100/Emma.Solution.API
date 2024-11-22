namespace Emma.Api.Models.DTOs
{
    public class FreightInformationsDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string StreetAndNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
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
