namespace Emma.Api.Models.DTOs
{
    public class VehicleManagementDTO
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public required string BodyType { get; set; }
        public string? PayloadCapacity { get; set; }
        public required string PlateNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
