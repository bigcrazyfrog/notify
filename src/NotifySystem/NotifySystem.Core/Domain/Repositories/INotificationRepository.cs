using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.SharedKernel.Storage;

namespace NotifySystem.Core.Domain.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<Notification>> GetPendingAsync(CancellationToken cancellationToken = default);
        Task<List<Notification>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default);
        Task<List<Notification>> GetFailedForRetryAsync(CancellationToken cancellationToken = default);
        Task<List<Notification>> GetScheduledAsync(DateTime beforeTime, CancellationToken cancellationToken = default);
        Task<List<Notification>> GetByTypeAsync(NotificationType type, CancellationToken cancellationToken = default);
        Task<List<Notification>> GetByRecipientAsync(long recipientId, CancellationToken cancellationToken = default);

        Task MarkAsSentAsync(long id, string channel, string providerResponse,
            CancellationToken cancellationToken = default);
        Task MarkAsFailedAsync(long id, string channel, string providerResponse,
            CancellationToken cancellationToken = default);
        Task MarkAsRetryingAsync(long id, CancellationToken cancellationToken = default);

        Task<List<(NotificationHistory History, Notification Notification)>> GetHistoryWithNotificationsAsync(
            long? notificationId = null,
            long? recipientId = null,
            string? channel = null,
            bool? isSuccess = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default);
    }
}
