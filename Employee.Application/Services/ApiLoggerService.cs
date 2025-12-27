using Employee.Application.Interfaces;
using System.Text;

namespace Employee.Application.Services
{
    public class ApiLoggerService : IApiLogger
    {
        private readonly string _logsDirectory;

        public ApiLoggerService()
        {
            // Check if running in Docker (logs are mapped to /app/logs)
            // Otherwise use Logs directory in project root
            var dockerLogsPath = "/app/logs";
            var projectRoot = Directory.GetCurrentDirectory();
            var localLogsPath = Path.Combine(projectRoot, "Logs");
            
            if (Directory.Exists("/app"))
            {
                // Running in Docker
                _logsDirectory = dockerLogsPath;
            }
            else
            {
                // Running locally
                _logsDirectory = localLogsPath;
            }
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(_logsDirectory))
            {
                Directory.CreateDirectory(_logsDirectory);
            }
        }

        public async Task LogApiCallAsync(string method, string path, string? queryString, int statusCode, long? responseTimeMs = null)
        {
            try
            {
                var timestamp = DateTime.Now;
                var fileName = $"API_Log_{timestamp:yyyy-MM-dd}.txt";
                var filePath = Path.Combine(_logsDirectory, fileName);

                var logEntry = new StringBuilder();
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine($"Timestamp: {timestamp:yyyy-MM-dd HH:mm:ss.fff}");
                logEntry.AppendLine($"HTTP Method: {method}");
                logEntry.AppendLine($"Path: {path}");
                
                if (!string.IsNullOrEmpty(queryString))
                {
                    logEntry.AppendLine($"Query String: {queryString}");
                }
                
                logEntry.AppendLine($"Status Code: {statusCode}");
                
                if (responseTimeMs.HasValue)
                {
                    logEntry.AppendLine($"Response Time: {responseTimeMs}ms");
                }
                
                logEntry.AppendLine("==========================================");
                logEntry.AppendLine();

                await File.AppendAllTextAsync(filePath, logEntry.ToString(), Encoding.UTF8);
            }
            catch (Exception ex)
            {
                // If logging fails, we don't want to crash the application
                // You can optionally log this to a separate error log
                Console.WriteLine($"Error writing API log: {ex.Message}");
            }
        }
    }
}

