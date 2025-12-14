using Microsoft.EntityFrameworkCore;
using NotifySystem.Application.Common.Logging;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Storage;
using NotifySystem.Infrastructure.Persistance.DataStorage;
using NotifySystem.Infrastructure.Persistance.DataStorage.Repositories;
using NotifySystem.Infrastructure.Persistance.Extensions;
using NotifySystem.Messaging.Extensions;
using NotifySystem.Messaging.Interfaces;
using EmailService.Application.Handlers;
using EmailService.Application.Interfaces;
using EmailService.Configuration;
using EmailService.RabbitMQ;
using EmailService.Services;
using NotifySystem.Core.Domain.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
  
var loggerConfig = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/email-service-.log", rollingInterval: RollingInterval.Day)
        .Enrich.WithProperty("Service", "EmailService")
        .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);
    
    
Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog();  
    
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddDbContext<NotifyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    
builder.Services.RegisterRepository<IRecipientRepository, IReadonlyRepository<Recipient>, RecipientRepository>();
builder.Services.RegisterRepository<INotificationRepository, IReadonlyRepository<Notification>, NotificationRepository>();

builder.Services.AddScoped<INotificationLogger, NotificationLogger>();
    
builder.Services.AddSingleton<EmailSender>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<ISendEmailNotificationHandler, SendEmailNotificationHandler>();

builder.Services.AddSingleton<IMessageProcessor, MessageProcessor>();
builder.Services.AddScoped<IRetryHandler, RetryHandler>();
builder.Services.AddScoped<INotificationStatusUpdater, NotificationStatusUpdater>();

builder.Services.AddRabbitMqMessaging(builder.Configuration, "Email Service");

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