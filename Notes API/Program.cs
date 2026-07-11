using WebApplication1.Splash;
using WebApplication1.Login;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Services
            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen();



            // API Services
            builder.Services.AddScoped<SplashAPI>();

            builder.Services.AddScoped<LoginAPI>();


            // JWT Services
            builder.Services.AddSingleton<TokenService>();

            builder.Services.AddSingleton<JwtCacheService>();



            var app = builder.Build();



            // Swagger

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



            // Health Check

            app.MapGet("/health", () =>
            {
                return "Notes API is running";
            })
            .WithName("Health");




            // Splash API

            app.MapPost("/splash",
                async (
                    SplashRequest request,
                    SplashAPI splashAPI) =>
                {
                    return await splashAPI.GetSplash(request);
                })
                .WithName("Splash")
                .WithOpenApi();





            // Login API

            app.MapPost("/login",
                (
                    LoginRequest request,
                    LoginAPI loginAPI) =>
                {
                    return loginAPI.Login(request);
                })
                .WithName("Login")
                .WithOpenApi();




            app.Run();
        }
    }
}