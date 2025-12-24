using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Employee.API.BackgroundServices
{
    public class GenericEventConsumer : BackgroundService
    {
        private readonly ILogger<GenericEventConsumer> _logger;
        private readonly IConfiguration _configuration;
        private readonly string[] _topics = { "client-registrations", "employee-events" };

        public GenericEventConsumer(ILogger<GenericEventConsumer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["KafkaSettings:BootstrapServers"],
                GroupId = "generic-event-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(_topics);

            _logger.LogInformation("Generic Kafka Consumer started, subscribing to topics: {Topics}", string.Join(", ", _topics));

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);
                    var message = $"[{DateTime.Now}] Topic: {consumeResult.Topic} | Message: {consumeResult.Message.Value}";
                    
                    _logger.LogInformation(message);
                    
                    // Ensure the logs directory exists (mapped via Docker volume)
                    var logDirectory = "logs";
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }

                    // Save every event to the mapped path
                    await File.AppendAllTextAsync(Path.Combine(logDirectory, "kafka_events.log"), message + Environment.NewLine, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message");
                }
            }

            consumer.Close();
        }
    }
}
