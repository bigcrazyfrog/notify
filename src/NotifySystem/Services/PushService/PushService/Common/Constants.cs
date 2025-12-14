namespace PushService.Common
{
    public static class NotificationChannels
    {
        public const string Push = "Push";
    }

    public static class NotificationMessages
    {
        public const string SentSuccessfully = "Sent successfully";
        public const string DefaultTitle = "Notification";
        public const int MaxRetries = 3;
    }

    public static class Platforms
    {
        public const string Android = "android";
        public const string iOS = "ios";
    }
}