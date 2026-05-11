using System.ComponentModel.DataAnnotations;

namespace MicroShop.ApiGateway.Configurations
{
    public class RedisCacheOptions
    {
        public const string SectionName = "Redis";

        public bool IsEnabled { get; init; } = true;

        [Required]
        public string ConnectionString { get; init; } = string.Empty;

        [Range(1, 86400)]
        public int DefaultTtlSeconds { get; init; } = 300;

        public string InstanceName { get; init; } =
            "MicroShopGateway:";
    }
}
