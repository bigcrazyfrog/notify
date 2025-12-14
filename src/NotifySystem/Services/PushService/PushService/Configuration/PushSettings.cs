namespace PushService.Configuration
{
    public class PushSettings
    {
        public FcmSettings Fcm { get; set; } = new();

        public void Validate()
        {
            Fcm.Validate();
        }
    }

    public class FcmSettings
    {
        public string ProjectId { get; set; } = string.Empty;
        public string ServiceAccountKeyPath { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;

        public void Validate()
        {
            if (!Enabled) return;

            if (string.IsNullOrWhiteSpace(ProjectId))
                throw new InvalidOperationException("FCM ProjectId is required");

            if (string.IsNullOrWhiteSpace(ServiceAccountKeyPath))
                throw new InvalidOperationException("FCM ServiceAccountKeyPath is required");

            if (!File.Exists(ServiceAccountKeyPath))
                throw new InvalidOperationException($"FCM Service Account Key file not found: {ServiceAccountKeyPath}");
        }
    }
}