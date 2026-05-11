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
    private const string _rateLimitLuaScript = @"
    -- KEYS[1]: کلید محدودسازی (مثلاً ip:127.0.0.1)
    -- ARGV[1]: حداکثر ظرفیت سطل (PermitLimit)
    -- ARGV[2]: تعداد توکن شارژ در هر دوره (TokensPerPeriod)
    -- ARGV[3]: مدت زمان دوره شارژ به ثانیه (ReplenishmentPeriodSeconds)
    -- ARGV[4]: زمان فعلی سرور به فرمت Unix Timestamp

    local key = KEYS[1]
    local tokenLimit = tonumber(ARGV[1])
    local refillRate = tonumber(ARGV[2])
    local refillPeriod = tonumber(ARGV[3])
    local currentTime = tonumber(ARGV[4])

    -- خواندن اطلاعات فعلی از هش‌مپ ردیس
    -- HMGET: دریافت مقادیر موجودی توکن و زمان آخرین شارژ
    local bucket = redis.call('HMGET', key, 'tokens', 'last_refill')
    local tokens = tonumber(bucket[1])
    local lastRefill = tonumber(bucket[2])

    -- اگر بار اول است، سطل را پر کن
    if tokens == nil then
        tokens = tokenLimit
        lastRefill = lastRefill + (math.floor(elapsed / refillPeriod) * refillPeriod)
    end

    -- محاسبه زمان سپری شده از آخرین شارژ
    local elapsed = math.max(0, currentTime - lastRefill)
    
    -- محاسبه تعداد توکن‌های جدید که باید اضافه شوند
    local refill = math.floor(elapsed / refillPeriod) * refillRate
    
    -- اضافه کردن توکن‌ها (اما نه بیشتر از سقف مجاز)
    tokens = math.min(tokenLimit, tokens + refill)

    -- اگر توکنی اضافه شد، زمان آخرین شارژ را بروزرسانی کن
    if refill > 0 then
        lastRefill = lastRefill + (math.floor(elapsed / refillPeriod) * refillPeriod)
    end

    -- بررسی اینکه آیا توکنی برای مصرف وجود دارد؟
    if tokens <= 0 then
        return { 0, 0 } -- اجازه عبور نده، موجودی صفر
    end

    -- کم کردن یک توکن برای درخواست فعلی
    tokens = tokens - 1

    -- ذخیره مقادیر جدید در ردیس
    redis.call('HMSET', key, 'tokens', tokens, 'last_refill', lastRefill)
    
    -- تنظیم زمان انقضا برای کلید جهت جلوگیری از پر شدن حافظه ردیس
    -- کلید دو برابر زمان شارژ زنده می‌ماند
    redis.call('EXPIRE', key, refillPeriod * 2)

    return { 1, tokens } -- اجازه عبور صادر شد (1) و مقدار باقی‌مانده برگشت داده شد
";
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