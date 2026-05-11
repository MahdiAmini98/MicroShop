using System.ComponentModel.DataAnnotations;

namespace MicroShop.ApiGateway.Configurations
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimit";

        [Range(1, 100000)]
        public int PermitLimit { get; init; } = 100;

        [Range(1, 1000)]
        public int TokensPerPeriod { get; init; } = 10;

        [Range(1, 3600)]
        public int ReplenishmentPeriodSeconds { get; init; } = 10;
    }
}
