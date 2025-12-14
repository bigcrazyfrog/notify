using Microsoft.Extensions.Logging;
using PushService.Common;
using PushService.DTOs;

namespace PushService.Services
{
    public class CompositePushSender : IPushNotificationSender
    {
        private readonly FcmPushSender _fcmSender;
        private readonly ILogger<CompositePushSender> _logger;

        public CompositePushSender(
            FcmPushSender fcmSender,
            ILogger<CompositePushSender> logger)
        {
            _fcmSender = fcmSender;
            _logger = logger;
        }

        public async Task<PushResult> SendAsync(SendPushNotificationRequest request)
        {
            _logger.LogInformation("Sending push notification to {Platform} device. NotificationId: {NotificationId}", 
                request.Platform, request.NotificationId);

            return request.Platform.ToLowerInvariant() switch
            {
                Platforms.Android => await _fcmSender.SendAsync(request),
                Platforms.iOS => await _fcmSender.SendAsync(request),
                _ => PushResult.Failure($"Unsupported platform: {request.Platform}")
            };
        }
    }
}