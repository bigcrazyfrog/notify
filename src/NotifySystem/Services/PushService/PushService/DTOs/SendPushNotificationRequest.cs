using PushService.Common;

namespace PushService.DTOs
{
    public class SendPushNotificationRequest
    {
        public SendPushNotificationRequest()
        {
        }

        public SendPushNotificationRequest(long notificationId, long recipientId, string? message)
        {
            NotificationId = notificationId;
            RecipientId = recipientId;
            Message = message;
        }

        public long NotificationId { get; set; }
        public long RecipientId { get; set; }
        public string? DeviceToken { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
        public string? Message { get; set; }
        public Dictionary<string, string>? Data { get; set; }
        public string Platform { get; set; } = Platforms.Android;
    }
}