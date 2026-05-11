using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Transforms;
using Microsoft.Extensions.Options;

namespace MicroShop.ApiGateway.Extensions;

public static class AuthForwardingExtensions
{
    public static IReverseProxyBuilder AddAuthForwardingTransforms(
        this IReverseProxyBuilder builder)
    {
        builder.AddTransforms(transformContext =>
        {
            var authOptions = transformContext.Services
                .GetRequiredService<IOptions<AuthForwardingOptions>>().Value;

            var mappingOptions = transformContext.Services
                .GetRequiredService<IOptions<ClaimsMappingOptions>>().Value;

            if (!authOptions.Enabled)
                return;

            // یه Transform واحد — نه دو callback
            transformContext.RequestTransforms.Add(
                new ClaimsToHeadersTransform(authOptions, mappingOptions));
        });

        return builder;
    }
}