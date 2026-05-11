namespace MicroShop.ApiGateway.Extensions
{
    public static class HostExtensions
    {
        public static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            return builder;
        }

        public static WebApplicationBuilder LoadCustomConfigurationFiles(this WebApplicationBuilder builder)
        {
            builder.Configuration.AddJsonFile(
                "ConfigurationFiles/reverse-proxy.json",
                optional: false,
                reloadOnChange: true);

            return builder;
        }
    }
}
