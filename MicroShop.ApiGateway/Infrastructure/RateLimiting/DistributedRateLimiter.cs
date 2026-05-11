using Microsoft.Extensions.Options;
using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Infrastructure.Redis;
using StackExchange.Redis;

namespace MicroShop.ApiGateway.Infrastructure.RateLimiting;

/// <summary>
/// این کلاس دقیقاً چه مشکلی را حل می‌کند؟
/// در معماری Microservices:
/// اگر ۵ instance از API Gateway داشته باشی:
/// هر instance شمارنده جدا دارد
/// پس کاربر می‌تواند: از هر instance سهمیه جدا بگیرد - عملاً rate limit را دور بزند
/// Redis چه مشکلی را حل می‌کند؟ Redis تبدیل می‌شود به:Single Source of Truth
/// تمام gatewayها: یک counter مشترک دارند state مشترک دارند
/// Redis Lua Script چه مزیتی دارد؟ Lua داخل خود Redis اجرا می‌شود: 
/// پس:سریع atomic thread-safe
/// تحلیل الگوریتم Token Bucket
/// مفهوم اصلی فرض کن یک سطل داری: Capacity = 100 token
/// هر request: 1 token مصرف می‌کند
/// Refill : هر 10 ثانیه به سطل اضافه میشه
/// 50 request ناگهانی میاد
/// اگر token داشته باشد: مجاز است.
/// Lua :یک زبان مخصوص Redis است که داخل خود Redis اجرا می‌شود
/// </summary>
public sealed class DistributedRateLimiter
    : IDistributedRateLimiter
{
    private readonly IDatabase _redis;
    private readonly RateLimitOptions _options;
    private const string _rateLimitLuaScript = """
local key = KEYS[1]

local tokenLimit = tonumber(ARGV[1])
local refillRate = tonumber(ARGV[2])
local refillPeriod = tonumber(ARGV[3])
local currentTime = tonumber(ARGV[4])

local bucket = redis.call('HMGET', key, 'tokens', 'last_refill')

local tokens = tonumber(bucket[1])
local lastRefill = tonumber(bucket[2])

if tokens == nil then
    tokens = tokenLimit
    lastRefill = currentTime
end

local elapsedTime = math.max(0, currentTime - lastRefill)

local refillTokens =
    math.floor(elapsedTime / refillPeriod) * refillRate

tokens = math.min(tokenLimit, tokens + refillTokens)

if refillTokens > 0 then
    lastRefill = currentTime
end

if tokens <= 0 then
    return {0, tokens}
end

tokens = tokens - 1

redis.call(
    'HMSET',
    key,
    'tokens',
    tokens,
    'last_refill',
    lastRefill
)

redis.call(
    'EXPIRE',
    key,
    refillPeriod * 2
)

return {1, tokens}
""";
    public DistributedRateLimiter(
        IRedisConnectionProvider redisProvider,
        IOptions<RateLimitOptions> options)
    {
        _redis = redisProvider.Database;
        _options = options.Value;
    }

    public async Task<RateLimitResult>
        IsRequestAllowedAsync(
            string key,
            CancellationToken cancellationToken = default)
    {
        var redisKey =
            $"rate_limit:{key}";

        var result = (RedisResult[]?)await _redis
            .ScriptEvaluateAsync(
                _rateLimitLuaScript,
                new RedisKey[] { redisKey },
                new RedisValue[]
                {
                    _options.PermitLimit,
                    _options.TokensPerPeriod,
                    _options.ReplenishmentPeriodSeconds,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

        if (result is null)
        {
            return new RateLimitResult
            {
                IsAllowed = false
            };
        }

        var allowed = (int)result[0] == 1;

        var remaining = (int)result[1];

        return new RateLimitResult
        {
            IsAllowed = allowed,
            RemainingTokens = remaining,
            RetryAfterSeconds =
                allowed
                    ? 0
                    : _options.ReplenishmentPeriodSeconds
        };
    }
}