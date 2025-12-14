using Microsoft.EntityFrameworkCore;
using NotifySystem.Application.Common.Logging;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.Services;
using NotifySystem.Core.Domain.SharedKernel.Storage;
using NotifySystem.Infrastructure.Persistance.DataStorage;
using NotifySystem.Infrastructure.Persistance.DataStorage.Repositories;
using NotifySystem.Infrastructure.Persistance.Extensions;
using NotifySystem.Messaging.Extensions;
using NotifySystem.Messaging.Interfaces;
using PushService.Application.Handlers;
using PushService.Application.Interfaces;
using PushService.Configuration;
using PushService.RabbitMQ;
using PushService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
  
var loggerConfig = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/push-service-.log", rollingInterval: RollingInterval.Day)
        .Enrich.WithProperty("Service", "PushService")
        .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);
    
Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();  
    
builder.Services.Configure<PushSettings>(
    builder.Configuration.GetSection("PushSettings"));
    
builder.Services.AddDbContext<NotifyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.RegisterRepository<IRecipientRepository, IReadonlyRepository<Recipient>, RecipientRepository>();
builder.Services.RegisterRepository<INotificationRepository, IReadonlyRepository<Notification>, NotificationRepository>();

builder.Services.AddScoped<INotificationLogger, NotificationLogger>();
    
builder.Services.AddSingleton<FcmPushSender>();
builder.Services.AddScoped<IPushNotificationSender, CompositePushSender>();
builder.Services.AddScoped<ISendPushNotificationHandler, SendPushNotificationHandler>();

builder.Services.AddSingleton<IMessageProcessor, MessageProcessor>();
builder.Services.AddScoped<IRetryHandler, RetryHandler>();
builder.Services.AddScoped<INotificationStatusUpdater, NotificationStatusUpdater>();

builder.Services.AddRabbitMqMessaging(builder.Configuration, "Push Service");

builder.Services.AddHostedService<RabbitMqHostedService>();
    
var app = builder.Build();

try
{
    Log.Information("Starting PushService");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "PushService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}