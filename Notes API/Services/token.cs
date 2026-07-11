using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication1.Services
{
    public class TokenService
    {

        private readonly string secretKey =
            "NotesAPISecretKey123456789012345";


        public string GenerateToken()
        {

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                );


            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );


            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    "NotesAPI"
                ),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };


            var token =
                new JwtSecurityToken(

                    issuer: "NotesAPI",

                    audience: "NotesApp",

                    claims: claims,

                    expires:
                    DateTime.UtcNow.AddHours(1),

                    signingCredentials: credentials
                );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);

        }
    }
}