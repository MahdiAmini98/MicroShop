namespace MicroShop.ApiGateway.Configurations
{
    public class RedisCacheOptions
    {
        public const string SectionName = "Redis";

        public string ConnectionString { get; set; } = string.Empty;
        public int DefaultTtlSeconds { get; set; } = 300;
        public bool IsEnabled { get; set; } = true;
    }
}
