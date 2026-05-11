namespace MicroShop.ApiGateway.Infrastructure.SwaggerEndpoints
{
    public interface ISwaggerEndpointService
    {
        IReadOnlyList<SwaggerEndpointInfo> GetEndpoints();
    }
    public sealed record SwaggerEndpointInfo(string Url, string Name);
}
