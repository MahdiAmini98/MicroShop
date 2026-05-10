namespace MicroShop.ApiGateway.Configurations
{
    public sealed class AuthForwardingOptions
    {
        public const string SectionName = "AuthForwarding";

        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Bearer token رو به upstream forward می‌کنه
        /// </summary>
        public bool ForwardBearerToken { get; init; } = true;

        /// <summary>
        /// اگه true باشه، به جای Authorization
        /// از CustomHeaderName استفاده می‌کنه
        /// مثال: X-Internal-Token برای internal service communication
        /// </summary>
        public bool UseCustomHeader { get; init; } = false;

        public string CustomHeaderName { get; init; } = "X-Internal-Token";

        /// <summary>
        /// Header که نشون می‌ده request از Gateway رد شده
        /// </summary>
        public string GatewayHeader { get; init; } = "X-Gateway-Name";
        public string GatewayHeaderValue { get; init; } = "MicroShop-Gateway";

        /// <summary>
        /// Header که وضعیت authentication رو نشون می‌ده
        /// </summary>
        public string IsAuthenticatedHeader { get; init; } = "X-Is-Authenticated";
    }

    /// <summary>
    /// تنظیمات تبدیل JWT Claims به Request Headers
    /// Section: "ClaimsMapping"
    /// 
    /// Key   = claim type در JWT
    /// Value = header name که به upstream فرستاده می‌شه
    /// </summary>
    public sealed class ClaimsMappingOptions
    {
        public const string SectionName = "ClaimsMapping";

        public bool Enabled { get; init; } = true;

        /// <summary>
        /// Map از claim type به header name
        /// claim هایی که اینجا نیستن، forward نمی‌شن
        /// </summary>
        public Dictionary<string, string> Mappings { get; init; } = new()
        {
            // JWT Standard Claims
            ["sub"] = "X-User-Id",
            ["name"] = "X-User-Name",
            ["email"] = "X-User-Email",
            ["preferred_username"] = "X-User-Username",

            // ASP.NET Core ClaimTypes
            ["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] = "X-User-Id",
            ["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] = "X-User-Name",
            ["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] = "X-User-Email"
        };

        /// <summary>
        /// Claim type هایی که به عنوان Role جمع‌آوری می‌شن
        /// همه مقادیرشون comma-separated در یه header فرستاده می‌شه
        /// </summary>
        public string[] RoleClaimTypes { get; init; } =
        [
            "role",
        "roles",
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ];

        /// <summary>
        /// Header name که roles توش فرستاده می‌شه
        /// </summary>
        public string RolesHeaderName { get; init; } = "X-User-Roles";

        /// <summary>
        /// Claim هایی که نباید forward بشن
        /// HashSet در runtime ساخته می‌شه — O(1) lookup
        /// </summary>
        public string[] ExcludedClaims { get; init; } =
        [
            "iat", "exp", "nbf", "aud", "iss",
        "password", "refresh_token",
        "nonce", "at_hash", "c_hash"
        ];
    }
}
