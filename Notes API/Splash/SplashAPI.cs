using Npgsql;
using WebApplication1.Services;


namespace WebApplication1.Splash
{
    public class SplashAPI
    {

        private readonly string _connectionString;

        private readonly TokenService _tokenService;

        private readonly JwtCacheService _jwtCache;



        public SplashAPI(
            IConfiguration configuration,
            TokenService tokenService,
            JwtCacheService jwtCache)
        {

            _connectionString =
                configuration
                .GetConnectionString("PostgresConnection")
                ?? string.Empty;


            _tokenService = tokenService;

            _jwtCache = jwtCache;

        }




        public async Task<IResult> GetSplash(
            SplashRequest request)
        {

            try
            {


                if (request == null)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Request cannot be empty"
                    });
                }



                if (request.Type != "SplashRequest")
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid type"
                    });
                }



                if (request.Ver <= 0)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid version"
                    });
                }



                using var conn =
                    new NpgsqlConnection(
                        _connectionString);


                await conn.OpenAsync();



                string sql = @"
                SELECT application_number,ver
                FROM splashData
                WHERE application_number=@applicationNumber";



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
                        Message = "Data not found"
                    });
                }


                var token =
                    _tokenService.GenerateToken();

                _jwtCache.SetToken(token);




                return Results.Ok(
                    new SplashResponse
                    {

                        JwtToken = token,


                        Data = new SplashData
                        {

                            Status = "Successful",

                            Message = "Splash loaded",


                            ApplicationNumber =
                            reader["application_number"]
                            .ToString()!,


                            Version =
                            reader["ver"]
                            .ToString()!

                        }

                    });


            }
            catch (Exception ex)
            {

                return Results.Problem(
                    ex.Message);

            }

        }

    }
}