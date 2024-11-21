namespace Emma.Api.Model
{
    public class Address
    {
        public int Id { get; set; }
        public required string StreetAndNumber { get; set; }
        public required string PostalCode { get; set; }
        public required string City { get; set; }
    }
}
