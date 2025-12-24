using Hangfire;
using Microsoft.Extensions.Logging;

namespace Employee.API.Jobs
{
    public class RecurringJobs
    {
        private readonly ILogger<RecurringJobs> _logger;

        public RecurringJobs(ILogger<RecurringJobs> logger)
        {
            _logger = logger;
        }

        public void Heartbeat()
        {
            _logger.LogInformation("System Heartbeat: All background services are running at {Time}", DateTime.Now);
        }

        public void DailyJob()
        {
            _logger.LogInformation("Executing Daily Job at {Time}", DateTime.Now);
        }

        public void WeeklyJob()
        {
            _logger.LogInformation("Executing Weekly Job at {Time}", DateTime.Now);
        }

        public void MonthlyJob()
        {
            _logger.LogInformation("Executing Monthly Job at {Time}", DateTime.Now);
        }

        public void YearlyJob()
        {
            _logger.LogInformation("Executing Yearly Job at {Time}", DateTime.Now);
        }
    }
}
