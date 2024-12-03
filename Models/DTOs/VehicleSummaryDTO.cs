namespace Emma.Api.Models.DTOs
{
    public class VehicleSummaryDTO
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public required string BodyType { get; set; }
        public string? PayloadCapacity { get; set; }
    }
}
