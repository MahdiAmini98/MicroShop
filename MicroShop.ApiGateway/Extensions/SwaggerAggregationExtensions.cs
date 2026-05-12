using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Infrastructure.SwaggerEndpoints;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

namespace MicroShop.ApiGateway.Extensions;

public static class SwaggerAggregationExtensions
{
    public static IServiceCollection AddSwaggerAggregation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = configuration
            .GetSection(SwaggerAggregationOptions.SectionName)
            .Get<SwaggerAggregationOptions>() ?? new SwaggerAggregationOptions();

        services.AddSingleton<ISwaggerEndpointService, SwaggerEndpointService>();

        if (!options.Enabled)
            return services;

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version,
                Description = options.Description
            });

            ConfigureJwtSecurity(swagger);
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerAggregation(
        this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<SwaggerAggregationOptions>>()
            .Value;

        if (!options.Enabled)
            return app;

        app.UseSwagger();

        app.UseSwaggerUI(swagger =>
        {
            swagger.RoutePrefix = options.RoutePrefix;

            swagger.SwaggerEndpoint(
                $"/swagger/{options.Version}/swagger.json",
                "ApiGateway");

            var endpointService = app.ApplicationServices
                .GetRequiredService<ISwaggerEndpointService>();

            foreach (var endpoint in endpointService.GetEndpoints())
            {
                swagger.SwaggerEndpoint(
                    endpoint.Url,
                    endpoint.Name);
            }

            swagger.DisplayRequestDuration();

            swagger.EnableDeepLinking();

            swagger.EnableFilter();

            swagger.DocExpansion(
                Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        });

        return app;
    }

    private static void ConfigureJwtSecurity(
        Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions swagger)
    {
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        };

        swagger.AddSecurityDefinition("Bearer", securityScheme);     
        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", null, null);

        var requirement = new OpenApiSecurityRequirement();
        requirement.Add(schemeReference, new List<string>());
        swagger.AddSecurityRequirement((doc) => requirement);
    }
}