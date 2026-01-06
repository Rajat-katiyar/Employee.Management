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

            Exception? exception = null;
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                exception = ex;

                // Write exception details to a daily exception log file (won't crash the app)
                try
                {
                    var projectRoot = Directory.GetCurrentDirectory();
                    var logsDir = Path.Combine(projectRoot, "Logs");
                    if (!Directory.Exists(logsDir)) Directory.CreateDirectory(logsDir);

                    var filePath = Path.Combine(logsDir, $"Exception_Log_{DateTime.Now:yyyy-MM-dd}.txt");
                    var entry = $"==========\nTimestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\nPath: {requestPath}\nMethod: {method}\nException: {ex}\n==========\n\n";
                    File.AppendAllText(filePath, entry);
                }
                catch { }

                throw;
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

                    // If there was an exception, also append a short marker to the API log
                    if (exception != null)
                    {
                        try
                        {
                            var projectRoot = Directory.GetCurrentDirectory();
                            var logsDir = Path.Combine(projectRoot, "Logs");
                            var filePath = Path.Combine(logsDir, $"API_Log_{DateTime.Now:yyyy-MM-dd}.txt");
                            var marker = $"-- Exception occurred: see Exception_Log_{DateTime.Now:yyyy-MM-dd}.txt --\n";
                            await File.AppendAllTextAsync(filePath, marker);
                        }
                        catch { }
                    }
                });
            }
        }
    }
}

