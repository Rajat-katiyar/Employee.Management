using Microsoft.Extensions.DependencyInjection;
using Employee.Application.Interfaces;
using Employee.Application.Services;

namespace Employee.Application
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IClientService, ClientService>();
            return services;
        }
    }
}
