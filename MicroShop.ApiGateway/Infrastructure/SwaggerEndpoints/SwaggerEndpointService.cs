using Yarp.ReverseProxy.Configuration;

namespace MicroShop.ApiGateway.Infrastructure.SwaggerEndpoints
{
    public sealed class SwaggerEndpointService
        : ISwaggerEndpointService
    {
        private const string SwaggerPrefix = "/swagger-json/";

        private readonly IProxyConfigProvider _proxyConfigProvider;

        public SwaggerEndpointService(IProxyConfigProvider proxyConfigProvider)
        {
            _proxyConfigProvider = proxyConfigProvider;
        }

        public IEnumerable<SwaggerEndpointInfo> GetEndpoints()
        {
            var routes = _proxyConfigProvider.GetConfig().Routes;
            return routes
                .Where(IsSwaggerRoute)
                .Select(CreateSwaggerEndpoint)
                .DistinctBy(x => x.Url)
                .OrderBy(x => x.Name);
        }

        private static bool IsSwaggerRoute(RouteConfig route)
        {
            return route.Match.Path?.StartsWith(
                SwaggerPrefix,
                StringComparison.OrdinalIgnoreCase) == true;
        }

        private static SwaggerEndpointInfo CreateSwaggerEndpoint(
            RouteConfig route)
        {
            var serviceKey = route.RouteId
                .Replace("-swagger-route", "");

            var displayName =
                $"{char.ToUpper(serviceKey[0])}{serviceKey[1..]}Service";

            var swaggerUrl = route.Match.Path!
                .Replace("{**catch-all}", "openapi/v1.json");

            return new SwaggerEndpointInfo(
                swaggerUrl,
                displayName);
        }
    }
}