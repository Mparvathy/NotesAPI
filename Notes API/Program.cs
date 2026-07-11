using WebApplication1.Login;
using WebApplication1.Register;
using WebApplication1.Services;
using WebApplication1.Splash;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<SplashAPI>();
            builder.Services.AddScoped<LoginAPI>();
            builder.Services.AddScoped<RegisterAPI>();
            builder.Services.AddSingleton<TokenService>();
            builder.Services.AddSingleton<JwtCacheService>();
            var app = builder.Build();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Notes API V1"
                );

                c.RoutePrefix = "swagger";
            });

            app.UseAuthorization();


            app.MapPost("/splash",
                async (
                    SplashRequest request,
                    SplashAPI splashAPI) =>
                {
                    return await splashAPI.GetSplash(request);
                })
                .WithName("Splash")
                .WithOpenApi();


            app.MapPost("/login",
                (
                    LoginRequest request,
                    LoginAPI loginAPI) =>
                {
                    return loginAPI.Login(request);
                })
                .WithName("Login")
                .WithOpenApi();

            app.MapPost("/Register",
               (
                   RegisterRequest request,
                   RegisterAPI  registerAPI) =>
               {
                   return registerAPI.Register(request);
               })
               .WithName("Register")
               .WithOpenApi();



            app.Run();
        }
    }
}