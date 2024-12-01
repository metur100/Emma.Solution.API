namespace Emma.Api.Models.DTOs
{
    public class VehicleInformationsDTO
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public required string UserName { get; set; }
        public required string StreetAndNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public string? Brand { get; set; }
        public string? PlateNumber { get; set; }
        public string? Description { get; set; }
        public required string BodyType { get; set; }
        public string? PayloadCapacity { get; set; }
        public string? CargoSpaceVolume { get; set; }
        public string? Length { get; set; }
        public string? Weight { get; set; }
    }
}
