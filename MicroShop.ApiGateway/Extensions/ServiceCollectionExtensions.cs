using MicroShop.ApiGateway.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MicroShop.ApiGateway.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReverseProxyWithTransforms(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddReverseProxy()
                    .LoadFromConfig(configuration.GetSection("ReverseProxy"))
                    .AddAuthForwardingTransforms();
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtOptions>()
                ?? throw new InvalidOperationException("Jwt section is missing in configuration.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = jwtSettings.Issuer,
                            ValidAudience = jwtSettings.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
                            ClockSkew = TimeSpan.Zero
                        };
                        // برای جلوگیری از خطای HTTPS در محیط توسعه
                        options.RequireHttpsMetadata = false;
                    });

            return services;
        }

        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AllowAnonymous", policy =>
                    policy.RequireAssertion(_ => true));
            });

            return services;
        }

        public static IServiceCollection AddCustomCors(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            var corsOptions = configuration
                .GetSection(CorsOptions.SectionName)
                .Get<CorsOptions>() ?? new CorsOptions();

            services.AddCors(options =>
            {
                options.AddPolicy("GatewayPolicy", policy =>
                {
                    // Origins
                    policy.WithOrigins(corsOptions.AllowedOrigins);

                    // Methods
                    policy.WithMethods(corsOptions.AllowedMethods);

                    // Headers
                    policy.WithHeaders(corsOptions.AllowedHeaders);

                    // Credentials
                    if (corsOptions.AllowCredentials)
                    {
                        policy.AllowCredentials();
                    }

                    // Preflight cache duration
                    policy.SetPreflightMaxAge(
                        TimeSpan.FromSeconds(corsOptions.PreflightMaxAgeSeconds));
                });
            });

            return services;
        }
    }
}
