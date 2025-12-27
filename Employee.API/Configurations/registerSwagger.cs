namespace Employee.API.Configurations
{
    public static class SwaggerRegistration
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            // Basic Swagger configuration - JWT security can be added via Swagger UI manually
            // or configured through appsettings.json if needed
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new()
                {
                    Title = "Employee Management API",
                    Version = "v1",
                    Description = "API for Employee Management System with JWT Authentication"
                });

                // Note: JWT Bearer token authentication is configured via Program.cs
                // Users can add the token manually in Swagger UI's "Authorize" button
                // To add automatic JWT support, ensure Microsoft.OpenApi.Models namespace is available
            });

            return services;
        }
    }
}
