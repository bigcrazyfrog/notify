using Microsoft.EntityFrameworkCore;
using NotifySystem.Application.Common.Logging;
using NotifySystem.Application.Features.Notifications;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.SharedKernel.Storage;
using NotifySystem.Messaging.Configuration;
using NotifySystem.Messaging.Interfaces;
using NotifySystem.Messaging.Publisher;
using NotifySystem.Infrastructure.Persistance.DataStorage;
using NotifySystem.Infrastructure.Persistance.DataStorage.Repositories;
using NotifySystem.Infrastructure.Persistance.Extensions;
using NotifySystem.Gateway.Middleware;
using Serilog;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .WriteTo.File("logs/notify-gateway-.log", rollingInterval: RollingInterval.Day)
    .Enrich.WithProperty("Service", "NotifyGateway")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName);

Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog((ctx, lc) =>
{
    lc.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddDbContext<NotifyDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.RegisterRepository<IRecipientRepository, IReadonlyRepository<Recipient>, RecipientRepository>();
builder.Services.RegisterRepository<INotificationRepository, IReadonlyRepository<Notification>, NotificationRepository>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(SendNotificationCommand).Assembly);
});

var rabbitMqSettings = new RabbitMqSettings();
builder.Configuration.GetSection("RabbitMQ").Bind(rabbitMqSettings);

builder.Services.AddSingleton<RabbitMqConnection>(sp =>
{
    return new RabbitMqConnection(
        rabbitMqSettings.Host, 
        rabbitMqSettings.Port, 
        rabbitMqSettings.Username, 
        rabbitMqSettings.Password, 
        rabbitMqSettings.VHost);
});

builder.Services.AddScoped<IEventPublisher, RabbitMqPublisher>(); 
builder.Services.AddScoped<INotificationLogger, NotificationLogger>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAll");
}

app.UseMiddleware<MetricsMiddleware>();         
app.MapMetrics("/metrics"); 

app.UseHttpsRedirection();
app.UseRouting();

app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
    };
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NotifyDbContext>();
    db.Database.Migrate();
}

try
{
    Log.Information("Starting NotifySystem Gateway");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
