using MicroShop.ApiGateway.Infrastructure.RateLimiting;
using MicroShop.ApiGateway.Middlewares;

namespace MicroShop.ApiGateway.Extensions
{
    public static class DistributedRateLimitingExtensions
    {
        public static IServiceCollection
            AddDistributedRateLimiting(
                this IServiceCollection services)
        {
            services.AddSingleton<
                IDistributedRateLimiter,
                DistributedRateLimiter>();

            return services;
        }

        public static IApplicationBuilder
            UseDistributedRateLimiting(this IApplicationBuilder app)
        {
            return app.UseMiddleware<DistributedRateLimitingMiddleware>();
        }
    }
}
