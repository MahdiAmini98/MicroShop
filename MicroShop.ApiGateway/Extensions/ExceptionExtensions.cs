using MicroShop.ApiGateway.Handlers;

namespace MicroShop.ApiGateway.Extensions
{
    public static class ExceptionExtensions
    {
        public static IServiceCollection AddGlobalExceptionHandling(
            this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }
    }
}
