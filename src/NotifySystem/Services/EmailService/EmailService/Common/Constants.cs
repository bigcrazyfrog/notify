namespace EmailService.Common
{
    public static class NotificationChannels
    {
        public const string Email = "Email";
    }

    public static class NotificationMessages
    {
        public const string SentSuccessfully = "Sent successfully";
        public const string DefaultSubject = "Notification";
        public const int MaxRetries = 3;
    }
}
