using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Middlewares;

namespace MicroShop.ApiGateway.Extensions
{
    public static class SecurityHeadersExtensions
    {
        public static IServiceCollection AddSecurityHeaders(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services
                .AddOptions<SecurityHeadersOptions>()
                .Bind(configuration.GetSection(SecurityHeadersOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            return app.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
