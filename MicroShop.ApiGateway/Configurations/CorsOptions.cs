using System.ComponentModel.DataAnnotations;

namespace MicroShop.ApiGateway.Configurations
{
    public class CorsOptions
    {
        public const string SectionName = "Cors";

        /// <summary>
        /// لیست origin های مجاز — در production هرگز "*" نباشه
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "At least one allowed origin must be specified.")]
        public string[] AllowedOrigins { get; init; } = [];

        public string[] AllowedMethods { get; init; } =
            ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"];

        public string[] AllowedHeaders { get; init; } =
            ["Content-Type", "Authorization", "X-Correlation-ID"];

        public bool AllowCredentials { get; init; } = false;

        [Range(0, 86400)]
        public int PreflightMaxAgeSeconds { get; init; } = 3600;
    }
}
