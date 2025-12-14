using PushService.DTOs;

namespace PushService.Services
{
    public interface IPushNotificationSender
    {
        Task<PushResult> SendAsync(SendPushNotificationRequest request);
    }

    public class PushResult
    {
        public bool IsSuccess { get; set; }
        public string? MessageId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ProviderResponse { get; set; }

        public static PushResult Success(string messageId, string? providerResponse = null)
            => new() { IsSuccess = true, MessageId = messageId, ProviderResponse = providerResponse };

        public static PushResult Failure(string errorMessage)
            => new() { IsSuccess = false, ErrorMessage = errorMessage };
    }
}