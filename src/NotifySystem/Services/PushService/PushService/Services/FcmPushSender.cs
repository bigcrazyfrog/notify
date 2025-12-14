using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PushService.Configuration;
using PushService.DTOs;

namespace PushService.Services
{
    public class FcmPushSender : IPushNotificationSender
    {
        private readonly FcmSettings _settings;
        private readonly ILogger<FcmPushSender> _logger;
        private readonly FirebaseApp? _firebaseApp;

        public FcmPushSender(IOptions<PushSettings> options, ILogger<FcmPushSender> logger)
        {
            var settings = options.Value;
            _settings = settings.Fcm;
            _logger = logger;

            if (!_settings.Enabled)
            {
                _logger.LogWarning("FCM is disabled in configuration");
                return;
            }

            try
            {
                var credential = GoogleCredential.FromFile(_settings.ServiceAccountKeyPath);
                    
                _firebaseApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential,
                    ProjectId = _settings.ProjectId
                }, "PushService");

                _logger.LogInformation("FCM initialized successfully for project: {ProjectId}", _settings.ProjectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize FCM");
                throw;
            }
        }

        public async Task<PushResult> SendAsync(SendPushNotificationRequest request)
        {
            if (!_settings.Enabled)
            {
                return PushResult.Failure("FCM is disabled");
            }

            if (_firebaseApp == null)
            {
                return PushResult.Failure("FCM is not initialized");
            }

            if (string.IsNullOrEmpty(request.DeviceToken))
            {
                return PushResult.Failure("Device token is required");
            }

            try
            {
                var message = new Message()
                {
                    Token = request.DeviceToken,
                    Notification = new Notification()
                    {
                        Title = request.Title ?? request.Message ?? "Notification",
                        Body = request.Body ?? request.Message ?? ""
                    },
                    Data = CreateDataDictionary(request),
                    Android = new AndroidConfig()
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification()
                        {
                            ChannelId = "default",
                            DefaultSound = true,
                            DefaultVibrateTimings = true
                        }
                    }
                };



                var messaging = FirebaseMessaging.GetMessaging(_firebaseApp);
                var response = await messaging.SendAsync(message);

                _logger.LogInformation("FCM message sent successfully. MessageId: {MessageId}, Token: {Token}", 
                    response, request.DeviceToken);

                return PushResult.Success(response, $"FCM Response: {response}");
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "FCM error sending to token {Token}: {ErrorCode} - {Message}", 
                    request.DeviceToken, ex.ErrorCode, ex.Message);
                
                return PushResult.Failure($"FCM Error: {ex.ErrorCode} - {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending FCM message to token {Token}", request.DeviceToken);
                return PushResult.Failure($"Unexpected error: {ex.Message}");
            }
        }

        private Dictionary<string, string> CreateDataDictionary(SendPushNotificationRequest request)
        {
            var data = request.Data != null 
                ? new Dictionary<string, string>(request.Data) 
                : new Dictionary<string, string>();
            
            data["notificationId"] = request.NotificationId.ToString();
            data["recipientId"] = request.RecipientId.ToString();

            return data;
        }
    }
}