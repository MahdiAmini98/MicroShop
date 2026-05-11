namespace MicroShop.ApiGateway.Infrastructure.RateLimiting
{
    public class RateLimitResult
    {
        public bool IsAllowed { get; init; }

        public int RemainingTokens { get; init; }

        public int RetryAfterSeconds { get; init; }
    }
}
