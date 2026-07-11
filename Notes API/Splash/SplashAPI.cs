using Npgsql;
using WebApplication1.Services;

namespace WebApplication1.Splash
{
    public class SplashAPI
    {
        private readonly string _connectionString;
        private readonly TokenService _tokenService;


        public SplashAPI(
            IConfiguration configuration,
            TokenService tokenService)
        {
            _connectionString =
                configuration.GetConnectionString("PostgresConnection")
                ?? string.Empty;

            _tokenService = tokenService;
        }



        public async Task<IResult> GetSplash(
            SplashRequest request)
        {
            try
            {
                // Request Validation
                if (request == null)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Request cannot be empty"
                    });
                }


                if (string.IsNullOrWhiteSpace(request.Type))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Type is required"
                    });
                }


                if (request.Type != "SplashRequest")
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid request type"
                    });
                }


                if (request.Ver <= 0)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Version is required"
                    });
                }


                if (string.IsNullOrWhiteSpace(request.ApplicationNumber))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Application number is required"
                    });
                }



                using var conn =
                    new NpgsqlConnection(_connectionString);


                await conn.OpenAsync();



                string sql = @"
                    SELECT application_number, ver
                    FROM splashData
                    WHERE application_number = @applicationNumber";



                using var cmd =
                    new NpgsqlCommand(sql, conn);


                cmd.Parameters.AddWithValue(
                    "@applicationNumber",
                    request.ApplicationNumber);



                using var reader =
                    await cmd.ExecuteReaderAsync();



                if (!await reader.ReadAsync())
                {
                    return Results.NotFound(new
                    {
                        Status = "Failed",
                        Message = "Application number not found"
                    });
                }



                // Generate JWT only on success
                var token =
                    _tokenService.GenerateToken();



                // ONLY HERE STATUS 200
                return Results.Ok(new SplashResponse
                {
                    JwtToken = token,

                    Data = new SplashData
                    {
                        Status = "Successful",

                        Message = "Splash loaded",

                        ApplicationNumber =
                            reader["application_number"]
                            .ToString(),

                        Version =
                            reader["ver"]
                            .ToString()
                    }
                });


            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Internal Server Error"
                );
            }
        }
    }
}