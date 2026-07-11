namespace WebApplication1.Services
{
    public class JwtCacheService
    {
        private string? _token;


        public void SetToken(string token)
        {
            _token = token;
        }


        public string? GetToken()
        {
            return _token;
        }


        public void RemoveToken()
        {
            _token = null;
        }
    }
}