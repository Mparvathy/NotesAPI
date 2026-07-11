namespace WebApplication1.Register
{
    public class RegisterRequest
    {
        public string Type { get; set; } = string.Empty;

        public int Ver { get; set; }

        public string JwtToken { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}