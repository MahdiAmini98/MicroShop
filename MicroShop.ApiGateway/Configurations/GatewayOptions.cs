using System.ComponentModel.DataAnnotations;

namespace MicroShop.ApiGateway.Configurations
{
    public class GatewayOptions
    {
        public const string SectionName = "Gateway";

        [Required(ErrorMessage = "Gateway name is required.")]
        public string Name { get; init; } = "MicroShop API Gateway";

        [Required]
        [Url(ErrorMessage = "BaseUrl must be a valid URL.")]
        public string BaseUrl { get; init; } = string.Empty;

        public bool EnableSwagger { get; init; } = true;

        public bool EnableDetailedErrors { get; init; } = false;
    }
}
