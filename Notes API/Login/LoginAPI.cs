using Npgsql;
using WebApplication1.Services;

namespace WebApplication1.Login
{
    public class LoginAPI
    {
        private readonly string _connectionString;
        private readonly JwtCacheService _jwtCache;
        private readonly TokenService _tokenService;

        public LoginAPI(
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

        public async Task<IResult> Login(LoginRequest request)
        {
            try
            {
       S
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

                if (request.Type != "LoginRequest")
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid request type"
                    });
                }

                // Version Validation
                if (request.Ver <= 0)
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Version is required"
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

                string sql = @"
                    SELECT
                        name,
                        username,
                        email,
                        phone
                    FROM users
                    WHERE username=@username
                    AND password=@password";

                using var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@username", request.Username);
                cmd.Parameters.AddWithValue("@password", request.Password);

                using var reader = await cmd.ExecuteReaderAsync();

                if (!await reader.ReadAsync())
                {
                    return Results.BadRequest(new
                    {
                        Status = "Failed",
                        Message = "Invalid username or password"
                    });
                }

                return Results.Ok(new LoginResponse
                {
                    JwtToken = request.JwtToken,

                    Data = new LoginData
                    {
                        Status = "Successful",
                        Message = "Login successful",
                        Name = reader["name"].ToString() ?? "",
                        Email = reader["email"].ToString() ?? "",
                        Phone = reader["phone"].ToString() ?? ""
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