using Employee.Application.Interfaces;
using System.Diagnostics;

namespace Employee.API.Middleware
{
    public class ApiLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApiLogger _apiLogger;

        public ApiLoggingMiddleware(RequestDelegate next, IApiLogger apiLogger)
        {
            _next = next;
            _apiLogger = apiLogger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip logging for Swagger and SignalR hubs
            var path = context.Request.Path.Value?.ToLower() ?? "";
            if (path.StartsWith("/swagger") || 
                path.StartsWith("/hangfire") ||
                path.StartsWith("/notificationhub") ||
                path == "/" ||
                path == "/favicon.ico")
            {
                await _next(context);
                return;
            }

            // Only log API endpoints
            if (!path.StartsWith("/api"))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var method = context.Request.Method;
            var requestPath = context.Request.Path.ToString();
            var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null;

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;
                var responseTimeMs = stopwatch.ElapsedMilliseconds;

                // Log asynchronously without blocking the response
                _ = Task.Run(async () =>
                {
                    await _apiLogger.LogApiCallAsync(
                        method,
                        requestPath,
                        queryString,
                        statusCode,
                        responseTimeMs
                    );
                });
            }
        }
    }
}

