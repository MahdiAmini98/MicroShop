namespace MicroShop.ApiGateway.Infrastructure.RateLimiting
{
    public interface IDistributedRateLimiter
    {
        Task<RateLimitResult> IsRequestAllowedAsync(
        string key,
        CancellationToken cancellationToken = default);
    }
}
