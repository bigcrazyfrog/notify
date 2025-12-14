using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.Enums;
using NotifySystem.Core.Domain.Repositories;
using NotifySystem.Core.Domain.Repositories.Specifications;
using NotifySystem.Core.Domain.SharedKernel.Specification;

namespace NotifySystem.Infrastructure.Persistance.DataStorage.Repositories
{
    public sealed class NotificationRepository : EFRepository<Notification, NotifyDbContext>, INotificationRepository
    {
        public NotificationRepository(NotifyDbContext context) : base(context)
        {
        }

        public async Task<List<Notification>> GetPendingAsync(CancellationToken cancellationToken = default)
        {
            return await Items
                .Include(n => n.History)
                .Where(n => n.Status == NotificationStatus.Pending)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Notification>> GetByStatusAsync(NotificationStatus status, CancellationToken cancellationToken = default)
        {
            return await Items.Where(n => n.Status == status).ToListAsync(cancellationToken);
        }

        public async Task MarkAsSentAsync(long id, string channel, string providerResponse, CancellationToken cancellationToken = default)
        {
            var notification = await Items
                .Include(n => n.History)
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
            
            if (notification != null)
            {
                notification.MarkAsSent(channel, providerResponse);
                await UpdateAsync(notification, cancellationToken);
            }
        }

        public async Task MarkAsFailedAsync(long id, string channel, string providerResponse, CancellationToken cancellationToken = default)
        {
            var notification = await Items
                .Include(n => n.History)
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
            
            if (notification != null)
            {
                notification.MarkAsFailed(channel, providerResponse);
                await UpdateAsync(notification, cancellationToken);
            }
        }

        public async Task MarkAsRetryingAsync(long id, CancellationToken cancellationToken = default)
        {
            var notification = await GetByIdAsync(id, cancellationToken);
            if (notification != null)
            {
                notification.MarkForRetry();
                await UpdateAsync(notification, cancellationToken);
            }
        }

        public async Task<List<Notification>> GetFailedForRetryAsync(CancellationToken cancellationToken = default)
        {
            return await Items
                .Where(n => n.Status == NotificationStatus.Failed)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Notification>> GetScheduledAsync(DateTime beforeTime, CancellationToken cancellationToken = default)
        {
            return await Items
                .Where(n => n.Status == NotificationStatus.Pending && n.CreatedAt <= beforeTime)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Notification>> GetByTypeAsync(NotificationType type, CancellationToken cancellationToken = default)
        {
            return await Items.Where(n => n.Type == type).ToListAsync(cancellationToken);
        }

        public async Task<List<Notification>> GetByRecipientAsync(long recipientId, CancellationToken cancellationToken = default)
        {
            return await Items.Where(n => n.RecipientId == recipientId).ToListAsync(cancellationToken);
        }

        public override async Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await Items
                .Include(n => n.History)
                .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
        }

        public async Task<List<(NotificationHistory History, Notification Notification)>> GetHistoryWithNotificationsAsync(
            long? notificationId = null,
            long? recipientId = null,
            string? channel = null,
            bool? isSuccess = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var specification = NotificationHistorySpecification.CombineFilters(
                notificationId, recipientId, channel, isSuccess, fromDate, toDate);

            var notifications = await Items
                .Include(n => n.History)
                .Where(specification.IsSatisfiedBy())
                .ToListAsync(cancellationToken);

            var result = notifications
                .SelectMany(n => n.History.Select(h => (History: h, Notification: n)))
                .Where(x => ApplyHistoryFilters(x.History, channel, isSuccess, fromDate, toDate))
                .OrderByDescending(x => x.History.AttemptedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return result;
        }

        private static bool ApplyHistoryFilters(
            NotificationHistory history,
            string? channel,
            bool? isSuccess,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (!string.IsNullOrEmpty(channel) && history.Channel != channel)
                return false;

            if (isSuccess.HasValue && history.IsSuccess != isSuccess.Value)
                return false;

            if (fromDate.HasValue && history.AttemptedAt < fromDate.Value)
                return false;

            if (toDate.HasValue && history.AttemptedAt > toDate.Value)
                return false;

            return true;
        }
    }
}
