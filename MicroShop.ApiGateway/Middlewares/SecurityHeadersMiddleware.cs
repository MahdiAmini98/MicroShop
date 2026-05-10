using MicroShop.ApiGateway.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace MicroShop.ApiGateway.Middlewares
{
    /// <summary>
    //Security Headers مجموعه‌ای از هدرهای HTTP هستند که به مرورگر و کلاینت‌ها دستورالعمل‌های امنیتی می‌دهند تا از حملات رایج وب جلوگیری شود.
    //جلوگیری از حملات مهم:
    ///XSS(Cross-Site Scripting)
    ///Clickjacking
    ///MIME Sniffing
    ///Insecure Mixed Content
    ///Information Disclosure
    /// </summary>


    /// <summary>
    /// Security Headers Middleware
    /// همه security headerها رو روی response اعمال می‌کنه
    /// باید بعد از GlobalExceptionMiddleware باشه
    /// تا روی error response‌ها هم اعمال بشه
    /// </summary>
    public sealed class SecurityHeadersMiddleware(
        RequestDelegate next,
        IOptions<SecurityHeadersOptions> options,
        IHostEnvironment env)
    {
        private readonly SecurityHeadersOptions _options = options.Value;
        private readonly IHostEnvironment _env = env; 

        public async Task InvokeAsync(HttpContext context)
        {
            // OnStarting: درست قبل از ارسال response header اجرا می‌شه
            // یه callback واحد — هم add هم remove اینجاست
            context.Response.OnStarting(() =>
            {
                if (!_options.Enabled)
                    return Task.CompletedTask;

                ApplySecurityHeaders(context);

                return Task.CompletedTask;
            });

            await next(context);
        }

        private void ApplySecurityHeaders(HttpContext context)
        {
            var headers = context.Response.Headers;

            // ── حذف header‌های حساس ──────────────────────────────
            if (_options.RemoveServerHeader)
            {
                headers.Remove("Server");
                headers.Remove("X-Powered-By");
                headers.Remove("X-AspNet-Version");
                headers.Remove("X-AspNetMvc-Version");
            }

            // ── افزودن Security Headers ───────────────────────────

            // جلوی MIME sniffing
            if (_options.ContentTypeOptionsNoSniff)
                headers["X-Content-Type-Options"] = "nosniff";

            // ضد Clickjacking
            if (!string.IsNullOrWhiteSpace(_options.XFrameOptions))
                headers["X-Frame-Options"] = _options.XFrameOptions;

            // کنترل Referer
            if (!string.IsNullOrWhiteSpace(_options.ReferrerPolicy))
                headers["Referrer-Policy"] = _options.ReferrerPolicy;

            // محدود کردن browser features
            if (!string.IsNullOrWhiteSpace(_options.PermissionsPolicy))
                headers["Permissions-Policy"] = _options.PermissionsPolicy;

            // CSP — null یعنی غیرفعال (Development)
            if (!string.IsNullOrWhiteSpace(_options.ContentSecurityPolicy))
                headers["Content-Security-Policy"] = _options.ContentSecurityPolicy;

            // HSTS — فقط HTTPS و غیر Development
            if (_options.EnableHsts && !_env.IsDevelopment() && context.Request.IsHttps)
                headers["Strict-Transport-Security"] = BuildHstsValue();

            // X-Request-ID برای tracing
            if (_options.AddRequestId)
                headers["X-Request-ID"] = context.TraceIdentifier;
        }

        private string BuildHstsValue()
        {
            var value = $"max-age={_options.HstsMaxAgeSeconds}";

            if (_options.HstsIncludeSubDomains)
                value += "; includeSubDomains";

            if (_options.HstsPreload)
                value += "; preload";

            return value;
        }
    }
}