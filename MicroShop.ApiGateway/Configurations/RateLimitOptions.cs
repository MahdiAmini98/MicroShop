using System.ComponentModel.DataAnnotations;

namespace MicroShop.ApiGateway.Configurations
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimit";

        [Range(1, 10000)]
        public int PermitLimit { get; set; } = 100;

        [Range(1, 3600)]
        public int WindowSeconds { get; set; } = 10;

        [Range(1, 1000)]
        public int QueueLimit { get; set; } = 0;
        public string Algorithm { get; set; } = "FixedWindow"; // FixedWindow, SlidingWindow, TokenBucket
        public bool Enabled { get; init; } = true;

    }
}
