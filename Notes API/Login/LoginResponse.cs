namespace WebApplication1.Login
{
    public class LoginResponse
    {

        public string JwtToken { get; set; } = string.Empty;


        public LoginData Data { get; set; } = new();

    }


    public class LoginData
    {

        public string Status { get; set; } = string.Empty;


        public string Message { get; set; } = string.Empty;


        public string Name { get; set; } = string.Empty;


        public string Username { get; set; } = string.Empty;


        public string Email { get; set; } = string.Empty;


        public string Phone { get; set; } = string.Empty;

    }
}