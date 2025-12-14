using System.Text;
using System.Text.Json;
using NotifySystem.Messaging.Interfaces;
using RabbitMQ.Client;

namespace NotifySystem.Messaging.Publisher;

public class RabbitMqPublisher : IEventPublisher
{
    private readonly RabbitMqConnection _connection;

    public RabbitMqPublisher(RabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, string queueName, CancellationToken cancellationToken = default) where TEvent : class
    {
        using var channel = await _connection.CreateChannelAsync();
        
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));
        
        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            body: body,
            mandatory: false
        );
    }
}