using Npgsql;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Render uses port 10000
            builder.WebHost.UseUrls("http://0.0.0.0:10000");


            // PostgreSQL Connection
            var connectionString = builder.Configuration
                .GetConnectionString("PostgresConnection");


            // Services
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();


            // Swagger Enable (Production + Development)
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    "/swagger/v1/swagger.json",
                    "Notes API V1"
                );

                c.RoutePrefix = "swagger";
            });


            app.UseHttpsRedirection();

            app.UseAuthorization();


            // Health Check
            app.MapGet("/", () =>
            {
                return "Notes API is running";
            });



            // POST Splash API
            app.MapPost("/splash", (SplashRequest request) =>
            {
                try
                {
                    using var conn = new NpgsqlConnection(connectionString);

                    conn.Open();


                    string sql = @"
                        SELECT application_number, ver
                        FROM splashData
                        WHERE application_number = @applicationNumber";


                    using var cmd = new NpgsqlCommand(sql, conn);


                    cmd.Parameters.AddWithValue(
                        "@applicationNumber",
                        request.ApplicationNumber
                    );


                    using var reader = cmd.ExecuteReader();


                    if (reader.Read())
                    {
                        return Results.Ok(new
                        {
                            ApplicationNumber =
                                reader["application_number"].ToString(),

                            Version =
                                reader["ver"].ToString()
                        });
                    }


                    return Results.NotFound(new
                    {
                        Message = "No data found"
                    });

                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        detail: ex.Message,
                        title: "Database Error"
                    );
                }

            })
            .WithName("Splash")
            .WithOpenApi();



            app.Run();
        }
    }



    // Request Model
    public class SplashRequest
    {
        public string ApplicationNumber { get; set; } = string.Empty;
    }
}
