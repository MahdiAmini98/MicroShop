using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace MicroShop.ApiGateway.Handlers
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var traceId = Activity.Current?.Id
                          ?? httpContext.TraceIdentifier;
            var statusCode = GetStatusCode(exception);

            _logger.LogError(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                traceId);

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = GetTitle(statusCode),
                Type = "https://httpstatuses.com/500",
                Instance = httpContext.Request.Path,
                Detail = GetDetail(exception, statusCode),
                Extensions =
            {
                ["traceId"] = traceId
            }
            };

            httpContext.Response.StatusCode =
                problemDetails.Status.Value;

            httpContext.Response.ContentType =
                "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);

            return true;
        }

        private int GetStatusCode(Exception exception) => exception switch
        {
            ArgumentException or ArgumentNullException or InvalidOperationException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            TimeoutException or OperationCanceledException => StatusCodes.Status408RequestTimeout,
            _ => StatusCodes.Status500InternalServerError
        };
        private string GetTitle(int statusCode) => statusCode switch
        {
            400 => "Bad Request",
            401 => "Unauthorized",
            404 => "Not Found",
            408 => "Request Timeout",
            500 => "Internal Server Error",
            _ => "An Error Occurred"
        };

        private string GetDetail(Exception ex, int statusCode)
        {
            if (_env.IsDevelopment())
                return ex.Message;

            return statusCode switch
            {
                500 => "An unexpected error occurred. Our team has been notified.",
                _ => ex.Message
            };
        }
    }
}