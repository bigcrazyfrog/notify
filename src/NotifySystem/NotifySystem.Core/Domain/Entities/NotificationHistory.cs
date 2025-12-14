using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotifySystem.Core.Domain.SharedKernel.Base;

namespace NotifySystem.Core.Domain.Entities
{
    public class NotificationHistory : Entity
    {
        public long NotificationId { get; private set; }
        public string Channel { get; private set; }
        public string? ProviderResponse { get; private set; }
        public DateTime AttemptedAt { get; private set; }
        public bool IsSuccess { get; private set; }
        public string? ErrorMessage { get; private set; }

        private NotificationHistory(
            long notificationId, 
            string channel, 
            string? providerResponse, 
            bool isSuccess, 
            string? errorMessage)
        {
            NotificationId = notificationId;
            Channel = channel;
            ProviderResponse = providerResponse;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            AttemptedAt = DateTime.UtcNow;
        }

        public static NotificationHistory CreateSuccess(long notificationId, string channel, string providerResponse)
            => new(notificationId, channel, providerResponse, true, null);

        public static NotificationHistory CreateFailure(long notificationId, string channel, string errorMessage)
            => new(notificationId, channel, null, false, errorMessage);
    }
}
