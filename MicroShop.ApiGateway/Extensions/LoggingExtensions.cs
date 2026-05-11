namespace MicroShop.ApiGateway.Extensions
{
    public static class LoggingExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                logger.LogInformation("Request: {Method} {Path} | TraceId: {TraceId}",
                    context.Request.Method, context.Request.Path, context.TraceIdentifier);

                await next();

                logger.LogInformation("Response: {StatusCode} | TraceId: {TraceId}",
                    context.Response.StatusCode, context.TraceIdentifier);
            });

            return app;
        }
    }
}
