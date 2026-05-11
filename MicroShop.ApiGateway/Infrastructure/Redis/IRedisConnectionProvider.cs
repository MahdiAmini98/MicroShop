using StackExchange.Redis;

namespace MicroShop.ApiGateway.Infrastructure.Redis
{
    public interface IRedisConnectionProvider
    {
        IConnectionMultiplexer Connection { get; }
        IDatabase Database { get; }
    }
}
