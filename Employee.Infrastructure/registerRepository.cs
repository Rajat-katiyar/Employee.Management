using Microsoft.Extensions.DependencyInjection;
using Employee.Application.Interfaces;
using Employee.Infrastructure.Repositories;

namespace Employee.Infrastructure
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IKafkaProducer, Employee.Infrastructure.Services.KafkaProducer>();
            return services;
        }
    }
}
