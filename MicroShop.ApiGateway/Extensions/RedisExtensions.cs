using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Infrastructure.Redis;

namespace MicroShop.ApiGateway.Extensions
{

    public static class RedisExtensions
    {
        public static IServiceCollection AddRedisInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddOptions<RedisCacheOptions>()
                .Bind(configuration.GetSection(
                    RedisCacheOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            var options = configuration
                .GetSection(RedisCacheOptions.SectionName)
                .Get<RedisCacheOptions>() ?? new();

            if (!options.IsEnabled)
                return services;

            // Connection Provider
            services.AddSingleton<IRedisConnectionProvider,
                RedisConnectionProvider>();

            // Distributed Cache
            services.AddStackExchangeRedisCache(redisOptions =>
            {
                redisOptions.Configuration =
                    options.ConnectionString;

                redisOptions.InstanceName =
                    options.InstanceName;
            });

            // Health Check
            services
                .AddHealthChecks()
                .AddCheck<RedisHealthCheck>("redis");

            return services;
        }
    }
}