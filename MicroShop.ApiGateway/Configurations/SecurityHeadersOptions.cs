using System.ComponentModel.DataAnnotations;

namespace MicroShop.ApiGateway.Configurations
{
    public sealed class SecurityHeadersOptions
    {

        public const string SectionName = "SecurityHeaders";

        /// <summary>
        /// فعال/غیرفعال کردن کل middleware
        /// در تست می‌شه خاموش کرد
        /// </summary>
        public bool Enabled { get; init; } = true;

        /// <summary>
        /// X-Content-Type-Options: nosniff
        /// جلوی MIME sniffing مرورگر رو می‌گیره
        /// </summary>
        public bool ContentTypeOptionsNoSniff { get; init; } = true;

        /// <summary>
        /// X-Frame-Options — ضد Clickjacking
        /// مقادیر معتبر: DENY | SAMEORIGIN
        /// </summary>
        [Required]
        public string XFrameOptions { get; init; } = "DENY";

        /// <summary>
        /// Referrer-Policy
        /// کنترل اطلاعات ارسالی در Referer header
        /// </summary>
        public string ReferrerPolicy { get; init; } = "strict-origin-when-cross-origin";

        /// <summary>
        /// Permissions-Policy — محدود کردن browser features
        /// برای API gateway نیازی به camera/mic/geolocation نیست
        /// </summary>
        public string PermissionsPolicy { get; init; } =
            "camera=(), microphone=(), geolocation=(), payment=()";

        /// <summary>
        /// Content-Security-Policy
        /// null = غیرفعال (مثلاً در Development برای Swagger)
        /// برای خالص API: "default-src 'none'; frame-ancestors 'none'"
        /// </summary>
        public string? ContentSecurityPolicy { get; init; } =
            "default-src 'none'; frame-ancestors 'none'";

        /// <summary>
        /// Strict-Transport-Security (HSTS)
        /// فقط در Production و روی HTTPS فعال کن
        /// </summary>
        public bool EnableHsts { get; init; } = false;

        [Range(0, 31536000)]
        public int HstsMaxAgeSeconds { get; init; } = 31536000;

        public bool HstsIncludeSubDomains { get; init; } = true;

        public bool HstsPreload { get; init; } = false;

        /// <summary>
        /// X-Request-ID به response اضافه می‌کنه
        /// فاز 4.1 (Correlation ID) ازش استفاده می‌کنه
        /// </summary>
        public bool AddRequestId { get; init; } = true;

        /// <summary>
        /// Server و X-Powered-By header رو حذف می‌کنه
        /// جلوی information leakage می‌گیره
        /// </summary>
        public bool RemoveServerHeader { get; init; } = true;
    }
}
