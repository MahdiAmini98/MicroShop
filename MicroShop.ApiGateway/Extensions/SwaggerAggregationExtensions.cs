using MicroShop.ApiGateway.Configurations;
using MicroShop.ApiGateway.Infrastructure.SwaggerEndpoints;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace MicroShop.ApiGateway.Extensions;

public static class SwaggerAggregationExtensions
{
    public static IServiceCollection AddSwaggerAggregation(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        // یکبار تنظیمات را بخوان
        var options = configuration
            .GetSection(SwaggerAggregationOptions.SectionName)
            .Get<SwaggerAggregationOptions>() ?? new();

        services
            .AddOptions<SwaggerAggregationOptions>()
            .Bind(configuration.GetSection(SwaggerAggregationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ISwaggerEndpointService, SwaggerEndpointService>();

        if (!options.Enabled)
            return services;

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(options.Version, new OpenApiInfo
            {
                Title = options.Title,
                Version = options.Version,
                Description = options.Description,
                Contact = new OpenApiContact { Name = "MicroShop Team" }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
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

        app.UseSwaggerUI(c =>
        {
            c.RoutePrefix = options.RoutePrefix;

            c.DocumentTitle = options.Title;

            c.DisplayRequestDuration();

            c.EnableDeepLinking();

            c.EnableFilter();

            c.EnableTryItOutByDefault();

            c.DefaultModelsExpandDepth(-1);

            // Gateway خودش
            c.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "API Gateway");

            // سرویس‌های upstream
            var endpointService = app.ApplicationServices
                .GetRequiredService<ISwaggerEndpointService>();

            foreach (var endpoint in endpointService.GetEndpoints())
            {
                c.SwaggerEndpoint(
                    endpoint.Url,
                    endpoint.Name);
            }
        });

        return app;
    }
}