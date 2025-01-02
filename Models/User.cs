namespace Emma.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public bool Confirmed { get; set; } = false;
        public string? ConfirmationToken { get; set; }
        public string? ResetPasswordToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
