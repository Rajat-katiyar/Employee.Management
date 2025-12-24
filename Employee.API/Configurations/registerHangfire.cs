using Hangfire;
using Employee.API.Jobs;

namespace Employee.API.Configurations
{
    public static class HangfireRegistration
    {
        public static IServiceCollection AddHangfireSetup(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();
            services.AddScoped<RecurringJobs>();

            return services;
        }

        public static IApplicationBuilder UseHangfireSetup(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire");

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                var jobs = scope.ServiceProvider.GetRequiredService<RecurringJobs>();

                recurringJobManager.AddOrUpdate("system-heartbeat", () => jobs.Heartbeat(), Cron.Minutely());
                recurringJobManager.AddOrUpdate("daily-job", () => jobs.DailyJob(), Cron.Daily());
                recurringJobManager.AddOrUpdate("weekly-job", () => jobs.WeeklyJob(), Cron.Weekly());
                recurringJobManager.AddOrUpdate("monthly-job", () => jobs.MonthlyJob(), Cron.Monthly());
                recurringJobManager.AddOrUpdate("yearly-job", () => jobs.YearlyJob(), Cron.Yearly());
            }

            return app;
        }
    }
}
