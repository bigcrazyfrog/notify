using Prometheus;

namespace NotifySystem.Gateway.Middleware;

public class MetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<MetricsMiddleware> _logger;
    
    private static readonly Histogram RequestDuration = Metrics
        .CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests", new[] { "method", "endpoint", "status_code" });

    private static readonly Counter RequestsTotal = Metrics
        .CreateCounter("http_requests_total", "Total number of HTTP requests", new[] { "method", "endpoint", "status_code" });

    public MetricsMiddleware(RequestDelegate next, ILogger<MetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var method = context.Request.Method;
        var endpoint = context.Request.Path.Value ?? "unknown";
        var statusCode = "0";
        
        using var timer = RequestDuration.WithLabels(method, endpoint, statusCode).NewTimer();
        
        try
        {
            await _next(context);
            
            statusCode = context.Response.StatusCode.ToString();
            RequestsTotal.WithLabels(method, endpoint, statusCode).Inc();
        }
        catch (Exception ex)
        {
            statusCode = "500";
            RequestsTotal.WithLabels(method, endpoint, statusCode).Inc();
            _logger.LogError(ex, "Request failed for {Method} {Endpoint}", method, endpoint);
            throw;
        }
        finally
        {
            RequestDuration.WithLabels(method, endpoint, statusCode).Observe(timer.ObserveDuration().TotalSeconds);
        }
    }
}