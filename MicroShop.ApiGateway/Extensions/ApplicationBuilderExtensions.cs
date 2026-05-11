namespace MicroShop.ApiGateway.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication ConfigurePipeline(this WebApplication app)
        {
            // Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerAggregation();
            }

            // Core Middlewares
            app.UseExceptionHandler();
            // Custom Middlewares
            app.UseRequestResponseLogging();
            app.UseHttpsRedirection(); 
            app.UseRouting();
            app.UseSecurityHeaders();
            app.UseCors("GatewayPolicy");
            app.UseGrpcWeb();

            // Security
            app.UseAuthentication();
            app.UseAuthorization();

            // Rate Limiting
            app.UseDistributedRateLimiting();

            // Endpoints
            app.MapHealthChecks("/health").AllowAnonymous();
            app.MapReverseProxy();

           

            // Test Endpoint (فقط Development)
            if (app.Environment.IsDevelopment())
            {
                app.MapTestTokenEndpoint();
            }

            return app;
        }
    }
}
