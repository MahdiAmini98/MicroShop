using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Middlewares;

namespace MicroShop.ApiGateway.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
