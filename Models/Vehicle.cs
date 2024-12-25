namespace Emma.Api.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public required string Brand { get; set; }
        public required string PlateNumber { get; set; }
        public string? Description { get; set; } 
        public required string BodyType { get; set; }
        public required string PayloadCapacity { get; set; }
        public string? CargoSpaceVolume { get; set; } 
        public string? Length { get; set; }
        public string? Weight { get; set; } 
        public bool IsActive { get; set; } = true;
    }
}
