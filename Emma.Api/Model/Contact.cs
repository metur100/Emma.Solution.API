namespace Emma.Api.Model
{
    public class Contact
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
    }
}
