namespace AuthService.Domain.DTOs
{
    public class RegisterDto
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
