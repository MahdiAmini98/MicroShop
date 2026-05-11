using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MicroShop.ApiGateway.Extensions
{
    public static class TestEndpointExtensions
    {
        public static IEndpointRouteBuilder MapTestTokenEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/generate-token", (HttpContext context) =>
            {
                var securityKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes("microshop-super-secret-key-minimum-32-characters-long-here"));

                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "MicroShopGateway",
                    audience: "MicroShopServices",
                    claims: new[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-123"),
                    new Claim(ClaimTypes.Name, "Test User"),
                    new Claim(ClaimTypes.Role, "Admin")
                    },
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { token = tokenString });
            });

            return endpoints;
        }
    }
}
