namespace MicroShop.ApiGateway.Infrastructure.SwaggerEndpoints
{
    public interface ISwaggerEndpointService
    {
        IEnumerable<SwaggerEndpointInfo> GetEndpoints();
    }
    public sealed record SwaggerEndpointInfo(string Url, string Name);
}
