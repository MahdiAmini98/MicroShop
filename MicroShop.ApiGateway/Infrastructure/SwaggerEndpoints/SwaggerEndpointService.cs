using MicroShop.ApiGateway.Configurations;
using Microsoft.Extensions.Options;
using Yarp.ReverseProxy.Configuration;

namespace MicroShop.ApiGateway.Infrastructure.SwaggerEndpoints
{
    public sealed class SwaggerEndpointService(
        IProxyConfigProvider proxyConfigProvider,
        IOptions<SwaggerAggregationOptions> options) : ISwaggerEndpointService
    {
        private readonly SwaggerAggregationOptions _options = options.Value;

        public IReadOnlyList<SwaggerEndpointInfo> GetEndpoints()
        {
            var config = proxyConfigProvider.GetConfig();

            return config.Routes
                .Where(r => r.Match?.Path?.StartsWith(
                    _options.SwaggerJsonPrefix,
                    StringComparison.OrdinalIgnoreCase) == true)
                .Select(BuildEndpointInfo)
                .OfType<SwaggerEndpointInfo>() // null ها رو حذف می‌کنه — جایگزین Where+Select
                .OrderBy(e => e.Name)
                .ToList()
                .AsReadOnly();
        }

        private SwaggerEndpointInfo? BuildEndpointInfo(RouteConfig route)
        {
            var path = route.Match?.Path;
            if (string.IsNullOrEmpty(path))
                return null;

            // مثال path: "/swagger-json/product/swagger/v1/swagger.json"
            // SwaggerJsonPrefix: "/swagger-json"
            // بعد از حذف prefix: "/product/swagger/v1/swagger.json"
            var afterPrefix = path[_options.SwaggerJsonPrefix.Length..]
                .TrimStart('/');

            // segments[0] = "product"
            var segments = afterPrefix.Split('/',
                StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
                return null;

            var serviceName = segments[0];

            // ✅ ToTitleCase بدون extension method
            var displayName = char.ToUpper(serviceName[0]) + serviceName[1..].ToLower();

            return new SwaggerEndpointInfo(
                Url: path,
                Name: $"{displayName} Service");
        }
    }
}