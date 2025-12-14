using RabbitMQ.Client;

namespace NotifySystem.Messaging.Publisher;

public class RabbitMqConnection : IDisposable
{
    private readonly IConnection _connection;
    private bool _disposed = false;

    public async Task<IChannel> CreateChannelAsync() => await _connection.CreateChannelAsync();

    public RabbitMqConnection(string host, int port, string username, string password, string vhost)
    {
        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password,
            VirtualHost = vhost
        };
        
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection?.Dispose();
            _disposed = true;
        }
    }
}