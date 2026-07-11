using Npgsql;
using WebApplication1.Services;

namespace WebApplication1.Register
{
    public class RegisterAPI
    {

        private readonly string _connectionString;
        private readonly JwtCacheService _jwtCache;
        private readonly TokenService _tokenService;

        public RegisterAPI(
            IConfiguration configuration,
            JwtCacheService jwtCache,
            TokenService tokenService)
        {
            _connectionString =
                configuration.GetConnectionString("PostgresConnection")
                ?? string.Empty;

            _jwtCache = jwtCache;
            _tokenService = tokenService;
        }

        public async Task<IResult> Register(RegisterRequest request)
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

                // Type Validation
                if (string.IsNullOrWhiteSpace(request.Type))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Type is required"
                    });
                }


                // Request  Validation
                if (request.Type != "RegisterRequest")
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid request type"
                    });
                }

                // Version Validation
                if (request.Ver != 1)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid API version"
                    });
                }


                // JWT Validation
                if (string.IsNullOrWhiteSpace(request.JwtToken))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "JWT Token is required"
                    });
                }

                var cachedToken = _jwtCache.GetToken();

                // JWT not found
                if (string.IsNullOrWhiteSpace(cachedToken))
                {
                    return Results.Json(
                        new
                        {
                            Status = "Failed",
                            Message = "JWT token not found. Call Splash API first."
                        },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
                }

                // JWT mismatch
                if (cachedToken != request.JwtToken)
                {
                    return Results.Json(
                        new
                        {
                            Status = "Failed",
                            Message = "Invalid JWT Token"
                        },
                        statusCode: StatusCodes.Status401Unauthorized
                    );
                }

                // Name Validation
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Name is required"
                    });
                }

                // Email Validation
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Email is required"
                    });
                }

                // Phone Validation
                if (string.IsNullOrWhiteSpace(request.Phone))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Phone is required"
                    });
                }

                // Username Validation
                if (string.IsNullOrWhiteSpace(request.Username))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Username is required"
                    });
                }

                // Password Validation
                if (string.IsNullOrWhiteSpace(request.Password))
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Password is required"
                    });
                }

                using var conn = new NpgsqlConnection(_connectionString);

                await conn.OpenAsync();

                string checkSql = @"
                        SELECT COUNT(*)
                        FROM users
                        WHERE username=@username
                        OR email=@email
                        OR phone=@phone";

                using var checkCmd = new NpgsqlCommand(checkSql, conn);

                checkCmd.Parameters.AddWithValue("@username", request.Username);
                checkCmd.Parameters.AddWithValue("@email", request.Email);
                checkCmd.Parameters.AddWithValue("@phone", request.Phone);

                int count = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());

                if (count > 0)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Username, Email or Phone already exists"
                    });
                }


                // Insert User

                                            string insertSql = @"
                            INSERT INTO users
                            (
                                name,
                                username,
                                email,
                                phone,
                                password
                            )
                            VALUES
                            (
                                @name,
                                @username,
                                @email,
                                @phone,
                                @password
                            )";

                using var insertCmd = new NpgsqlCommand(insertSql, conn);

                insertCmd.Parameters.AddWithValue("@name", request.Name);
                insertCmd.Parameters.AddWithValue("@username", request.Username);
                insertCmd.Parameters.AddWithValue("@email", request.Email);
                insertCmd.Parameters.AddWithValue("@phone", request.Phone);
                insertCmd.Parameters.AddWithValue("@password", request.Password);

                await insertCmd.ExecuteNonQueryAsync();

                var newToken = _tokenService.GenerateToken();

                _jwtCache.SetToken(newToken);


                return Results.Ok(new RegisterResponse
                {
                    JwtToken = newToken,

                    Data = new RegisterData
                    {
                        Status = "Successful",
                        Message = "Register successful",
                        Name = request.Name,
                        Username = request.Username,
                        Email = request.Email,
                        Phone = request.Phone
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    title: "Internal Server Error");
            }
        }
    }
}
