namespace NotifySystem.Messaging.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, string queueName, CancellationToken cancellationToken = default) where TEvent : class;
}