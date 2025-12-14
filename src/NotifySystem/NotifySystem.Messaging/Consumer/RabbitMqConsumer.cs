using Microsoft.Extensions.Logging;
using NotifySystem.Messaging.Configuration;
using NotifySystem.Messaging.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NotifySystem.Messaging.Consumer;

public class RabbitMqConsumer : IRabbitMqConsumer
{
    private readonly RabbitMqSettings _settings;
    private readonly IMessageProcessor _messageProcessor;
    private readonly ILogger<RabbitMqConsumer> _logger;
    private readonly string _serviceName;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumer(
        RabbitMqSettings settings,
        IMessageProcessor messageProcessor,
        ILogger<RabbitMqConsumer> logger,
        string serviceName)
    {
        _settings = settings;
        _messageProcessor = messageProcessor;
        _logger = logger;
        _serviceName = serviceName;
    }

    public async Task StartListeningAsync(CancellationToken cancellationToken)
    {
        try
        {
            await InitializeConnectionAsync();
            await SetupConsumerAsync();

            _logger.LogInformation("{ServiceName} listening to RabbitMQ queue '{QueueName}' on {Host}:{Port}",
                _serviceName, _settings.QueueName, _settings.Host, _settings.Port);

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ connection error: {Message}", ex.Message);
        }
        finally
        {
            await DisposeAsync();
        }
    }

    private async Task InitializeConnectionAsync()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.Host,
                Port = _settings.Port,
                UserName = _settings.Username,
                Password = _settings.Password,
                VirtualHost = _settings.VHost
            };

            _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port} with user {Username}", 
                _settings.Host, _settings.Port, _settings.Username);

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            _logger.LogInformation("RabbitMQ connection established, declaring queue '{QueueName}'", _settings.QueueName);
            
            await _channel.QueueDeclareAsync(
                queue: _settings.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false);
            
            _logger.LogInformation("RabbitMQ queue '{QueueName}' declared successfully", _settings.QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection: {Message}", ex.Message);
            throw;
        }
    }

    private async Task SetupConsumerAsync()
    {
        if (_channel == null)
            throw new InvalidOperationException("Channel is not initialized");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += OnMessageReceivedAsync;

        var consumerTag = await _channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false,
            consumerTag: "",
            consumer: consumer);

        _logger.LogInformation("Consumer started with tag '{ConsumerTag}' for queue '{QueueName}'", 
            consumerTag, _settings.QueueName);
    }

    private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            _logger.LogInformation("Received message with delivery tag {DeliveryTag} from queue '{QueueName}'", 
                ea.DeliveryTag, _settings.QueueName);

            var processed = await _messageProcessor.ProcessMessageAsync(ea);
            
            if (_channel != null)
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
                _logger.LogInformation("Message {DeliveryTag} acknowledged, processed: {Processed}", 
                    ea.DeliveryTag, processed);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in message consumer: {Message}", ex.Message);
            
            if (_channel != null)
            {
                await _channel.BasicAckAsync(ea.DeliveryTag, false);
                _logger.LogWarning("Message {DeliveryTag} acknowledged after error", ea.DeliveryTag);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }
        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
        _logger.LogInformation("RabbitMQ connection closed for {ServiceName}", _serviceName);
    }
}