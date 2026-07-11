using WebApplication1.Splash;
using WebApplication1.Services;

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

            builder.Services.AddSingleton<TokenService>();


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



            app.MapGet("/", () =>
            {
                return "Notes API is running";
            });



            app.MapPost("/splash",
                async (
                    SplashRequest request,
                    SplashAPI splashAPI) =>
                {
                    return await splashAPI.GetSplash(request);
                })
                .WithName("Splash")
                .WithOpenApi();



            app.Run();
        }
    }
}