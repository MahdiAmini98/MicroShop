using MicroShop.ApiGateway.Infrastructure.RateLimiting;

namespace MicroShop.ApiGateway.Middlewares
{
    public sealed class DistributedRateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        public DistributedRateLimitingMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IDistributedRateLimiter limiter)
        {
            var key = GetPartitionKey(context);

            var result =
                await limiter.IsRequestAllowedAsync(key);

            if (!result.IsAllowed)
            {
                context.Response.StatusCode = 429;

                context.Response.Headers.RetryAfter =
                    result.RetryAfterSeconds.ToString();

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Too many requests",
                    retryAfter =
                        result.RetryAfterSeconds
                });

                return;
            }

            context.Response.Headers.Append(
                "X-RateLimit-Remaining",
                result.RemainingTokens.ToString());

            await _next(context);
        }

        private static string GetPartitionKey(
            HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                return $"user:{context.User.Identity.Name}";
            }

            return $"ip:{context.Connection.RemoteIpAddress}";
        }
    }
}
