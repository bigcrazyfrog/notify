using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NotifySystem.Messaging.Configuration;
using NotifySystem.Messaging.Consumer;
using NotifySystem.Messaging.Interfaces;
using NotifySystem.Messaging.Publisher;

namespace NotifySystem.Messaging.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMqMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        string serviceName,
        string configSectionName = "RabbitMq")
    {
        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection(configSectionName).Bind(rabbitMqSettings);
        rabbitMqSettings.Validate();
        
        services.AddSingleton(rabbitMqSettings);
        services.Configure<RabbitMqSettings>(opts => configuration.GetSection(configSectionName).Bind(opts));
        
        services.AddSingleton<RabbitMqConnection>(sp =>
        {
            return new RabbitMqConnection(
                rabbitMqSettings.Host,
                rabbitMqSettings.Port,
                rabbitMqSettings.Username,
                rabbitMqSettings.Password,
                rabbitMqSettings.VHost);
        });

        services.AddScoped<IEventPublisher, RabbitMqPublisher>();

        services.AddSingleton<IRabbitMqConsumer>(provider =>
        {
            var messageProcessor = provider.GetRequiredService<IMessageProcessor>();
            var logger = provider.GetRequiredService<ILogger<RabbitMqConsumer>>();
            return new RabbitMqConsumer(rabbitMqSettings, messageProcessor, logger, serviceName);
        });

        return services;
    }
}