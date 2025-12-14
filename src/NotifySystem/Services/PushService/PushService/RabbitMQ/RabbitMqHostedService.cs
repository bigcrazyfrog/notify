using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotifySystem.Messaging.Interfaces;

namespace PushService.RabbitMQ
{
    public class RabbitMqHostedService : BackgroundService
    {
        private readonly IRabbitMqConsumer _consumer;
        private readonly ILogger<RabbitMqHostedService> _logger;

        public RabbitMqHostedService(IRabbitMqConsumer consumer, ILogger<RabbitMqHostedService> logger)
        {
            _consumer = consumer;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RabbitMQ Hosted Service starting...");
            
            try
            {
                await _consumer.StartListeningAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RabbitMQ Hosted Service: {Message}", ex.Message);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("RabbitMQ Hosted Service stopping...");
            await _consumer.DisposeAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}