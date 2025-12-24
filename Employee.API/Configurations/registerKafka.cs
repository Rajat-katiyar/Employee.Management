using Employee.API.BackgroundServices;

namespace Employee.API.Configurations
{
    public static class KafkaRegistration
    {
        public static IServiceCollection AddKafkaBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<GenericEventConsumer>();
            return services;
        }
    }
}
