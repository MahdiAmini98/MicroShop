namespace MicroShop.ApiGateway.Configurations
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; } = 60;
        public bool RequireHttpsMetadata { get; init; } = true;

        // اعتبارسنجی سفارشی
        public bool IsValid() =>
            !string.IsNullOrWhiteSpace(Key) &&
            Key.Length >= 32 &&
            !string.IsNullOrWhiteSpace(Issuer) &&
            !string.IsNullOrWhiteSpace(Audience);
    }
}
