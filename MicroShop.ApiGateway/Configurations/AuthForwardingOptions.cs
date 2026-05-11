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

        public Dictionary<string, string> Mappings { get; set; } = new();

        public string[] RoleClaimTypes { get; set; } =
        [
            "role",
        "roles",
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ];

        public string RolesHeaderName { get; init; } = "X-User-Roles";

        public string[] ExcludedClaims { get; set; } =
        [
            "iat",
        "exp",
        "nbf",
        "aud",
        "iss",
        "password",
        "refresh_token",
        "nonce",
        "at_hash",
        "c_hash"
        ];
    }
}
