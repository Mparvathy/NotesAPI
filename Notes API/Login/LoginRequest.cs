namespace WebApplication1.Login
{
    public class LoginRequest
    {

        public string Type { get; set; }
            = string.Empty;


        public int Ver { get; set; }


        public string JwtToken { get; set; }
            = string.Empty;


        public string Username { get; set; }
            = string.Empty;


        public string Password { get; set; }
            = string.Empty;

    }
}