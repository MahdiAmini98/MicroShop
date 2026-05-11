using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MicroShop.ApiGateway.Infrastructure.Redis
{
    public sealed class RedisHealthCheck
     : IHealthCheck
    {
        private readonly IRedisConnectionProvider _redis;

        public RedisHealthCheck(
            IRedisConnectionProvider redis)
        {
            _redis = redis;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var ping = await _redis
                    .Database
                    .PingAsync();

                return HealthCheckResult.Healthy(
                    $"Redis OK - Ping: {ping.TotalMilliseconds} ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Redis is unavailable",
                    ex);
            }
        }
    }
}