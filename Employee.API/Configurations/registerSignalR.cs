using Employee.API.Hubs;

namespace Employee.API.Configurations
{
    public static class SignalRRegistration
    {
        public static IServiceCollection AddSignalRSetup(this IServiceCollection services)
        {
            services.AddSignalR();
            return services;
        }

        public static IEndpointRouteBuilder MapSignalRHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<NotificationHub>("/notificationHub");
            return endpoints;
        }
    }
}
