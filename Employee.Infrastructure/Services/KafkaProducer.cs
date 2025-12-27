using Confluent.Kafka;
using Employee.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Employee.Infrastructure.Services
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, string>? _producer;
        private readonly ILogger<KafkaProducer> _logger;
        private readonly bool _isEnabled;

        public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
        {
            _logger = logger;
            var bootstrapServers = configuration["KafkaSettings:BootstrapServers"];

            if (string.IsNullOrEmpty(bootstrapServers))
            {
                _logger.LogWarning("Kafka is not configured. Messages will not be published.");
                _isEnabled = false;
                return;
            }

            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = bootstrapServers,
                    MessageTimeoutMs = 5000 // 5 second timeout
                };

                _producer = new ProducerBuilder<Null, string>(config).Build();
                _isEnabled = true;
                _logger.LogInformation("Kafka Producer initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Kafka Producer. Messages will not be published.");
                _isEnabled = false;
            }
        }

        public async Task ProduceAsync(string topic, string message)
        {
            if (!_isEnabled || _producer == null)
            {
                _logger.LogWarning("Kafka is disabled. Skipping message: {Message}", message);
                return;
            }

            try
            {
                await _producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
                _logger.LogInformation("Message published to Kafka topic {Topic}: {Message}", topic, message);
            }
            catch (ProduceException<Null, string> ex)
            {
                _logger.LogError(ex, "Failed to publish message to Kafka topic {Topic}. Error: {Error}", topic, ex.Error.Reason);
            }
        }
    }
}
