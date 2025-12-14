using RabbitMQ.Client.Events;

namespace NotifySystem.Messaging.Interfaces;

public interface IMessageProcessor
{
    Task<bool> ProcessMessageAsync(BasicDeliverEventArgs eventArgs);
}