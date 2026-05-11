namespace MicroShop.ApiGateway.Configurations
{
    public class SwaggerAggregationOptions
    {
        public const string SectionName = "SwaggerAggregation";

        public bool Enabled { get; init; } = true;
        public string RoutePrefix { get; init; } = "swagger";

        public string Title { get; init; } = "MicroShop API Gateway";
        public string Version { get; init; } = "v1";
        public string Description { get; init; } = "Unified API Documentation for All Microservices";

        public bool RequireAuthentication { get; init; } = false;

        /// <summary>
        /// پیشوند مسیرهایی که YARP برای swagger proxy می‌کنه
        /// باید با swagger-json شروع بشه تا با Swagger UI conflict نداشته باشه
        /// </summary>
        public string SwaggerJsonPrefix { get; init; } = "/swagger-json";
    }
}
