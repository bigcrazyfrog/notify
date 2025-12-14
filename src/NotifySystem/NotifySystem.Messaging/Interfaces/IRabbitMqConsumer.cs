namespace NotifySystem.Messaging.Interfaces;

public interface IRabbitMqConsumer : IAsyncDisposable
{
    Task StartListeningAsync(CancellationToken cancellationToken);
}