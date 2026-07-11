namespace WebApplication1.Splash
{
    public class SplashResponse
    {

        public string JwtToken { get; set; }
            = string.Empty;


        public SplashData Data { get; set; }
            = new();

    }



    public class SplashData
    {

        public string Status { get; set; }
            = string.Empty;


        public string Message { get; set; }
            = string.Empty;


        public string ApplicationNumber { get; set; }
            = string.Empty;


        public string Version { get; set; }
            = string.Empty;

    }
}