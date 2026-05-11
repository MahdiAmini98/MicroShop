using MicroShop.ApiGateway.Configurations;

namespace MicroShop.ApiGateway.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddStrongConfiguration(
         this IServiceCollection services,
         IConfiguration configuration)
        {


            services
            .AddOptions<AuthForwardingOptions>()
            .Bind(configuration.GetSection(AuthForwardingOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

            services
                .AddOptions<ClaimsMappingOptions>()
                .Bind(configuration.GetSection(ClaimsMappingOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();



            services
                .AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<CorsOptions>()
                .Bind(configuration.GetSection(CorsOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<GatewayOptions>()
                .Bind(configuration.GetSection(GatewayOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<RateLimitOptions>()
                .Bind(configuration.GetSection(RateLimitOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<RedisCacheOptions>()
                .Bind(configuration.GetSection(RedisCacheOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services
                .AddOptions<SecurityHeadersOptions>()
                .Bind(configuration.GetSection(SecurityHeadersOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();


            services
                .AddOptions<SwaggerAggregationOptions>()
                .Bind(configuration.GetSection(SwaggerAggregationOptions.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();




            return services;
        }
    }
}
