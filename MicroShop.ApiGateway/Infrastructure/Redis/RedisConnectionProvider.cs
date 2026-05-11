using Microsoft.Extensions.Options;
using MicroShop.ApiGateway.Configurations;
using StackExchange.Redis;

namespace MicroShop.ApiGateway.Infrastructure.Redis;

public sealed class RedisConnectionProvider
    : IRedisConnectionProvider, IDisposable
{
    private readonly Lazy<ConnectionMultiplexer>
        _lazyConnection;

    public RedisConnectionProvider(
        IOptions<RedisCacheOptions> options)
    {
        var redisOptions = options.Value;

        // Parse استاندارد connection string
        var configuration =
            ConfigurationOptions.Parse(
                redisOptions.ConnectionString);

        // Production-safe settings
        configuration.AbortOnConnectFail = false;

        configuration.ConnectRetry = 3;

        configuration.ConnectTimeout = 5000;

        configuration.SyncTimeout = 5000;

        // Lazy:
        // تا زمانی که Redis واقعاً نیاز نشود
        // connection ساخته نمی‌شود
        _lazyConnection =
            new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(configuration));
    }

    /// <summary>
    /// Singleton Redis connection
    /// </summary>
    public IConnectionMultiplexer Connection =>
        _lazyConnection.Value;

    /// <summary>
    /// Redis database accessor
    /// </summary>
    public IDatabase Database =>
        Connection.GetDatabase();

    public void Dispose()
    {
        if (_lazyConnection.IsValueCreated)
        {
            _lazyConnection.Value.Dispose();
        }
    }
}