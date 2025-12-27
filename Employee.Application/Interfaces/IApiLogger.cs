namespace Employee.Application.Interfaces
{
    public interface IApiLogger
    {
        Task LogApiCallAsync(string method, string path, string? queryString, int statusCode, long? responseTimeMs = null);
    }
}

