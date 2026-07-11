namespace WebApplication1.Register
{
    public class RegisterResponse
    {
        public string JwtToken { get; set; } = string.Empty;

        public RegisterData Data { get; set; } = new();
    }

    public class RegisterData
    {
        public string Status { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
    }
}