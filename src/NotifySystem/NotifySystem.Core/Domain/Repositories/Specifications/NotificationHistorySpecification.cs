using NotifySystem.Core.Domain.Entities;
using NotifySystem.Core.Domain.SharedKernel.Specification;

namespace NotifySystem.Core.Domain.Repositories.Specifications;

public static class NotificationHistorySpecification
{
    public static ISpecification<Notification> ByNotificationId(long notificationId)
    {
        return Specification<Notification>.Create(n => n.Id == notificationId);
    }

    public static ISpecification<Notification> ByRecipientId(long recipientId)
    {
        return Specification<Notification>.Create(n => n.RecipientId == recipientId);
    }

    public static ISpecification<Notification> HasHistoryWithChannel(string channel)
    {
        return Specification<Notification>.Create(n => n.History.Any(h => h.Channel == channel));
    }

    public static ISpecification<Notification> HasHistoryWithSuccess(bool isSuccess)
    {
        return Specification<Notification>.Create(n => n.History.Any(h => h.IsSuccess == isSuccess));
    }

    public static ISpecification<Notification> HasHistoryFromDate(DateTime fromDate)
    {
        return Specification<Notification>.Create(n => n.History.Any(h => h.AttemptedAt >= fromDate));
    }

    public static ISpecification<Notification> HasHistoryToDate(DateTime toDate)
    {
        return Specification<Notification>.Create(n => n.History.Any(h => h.AttemptedAt <= toDate));
    }

    public static ISpecification<Notification> HasHistory()
    {
        return Specification<Notification>.Create(n => n.History.Any());
    }

    public static ISpecification<Notification> CombineFilters(
        long? notificationId = null,
        long? recipientId = null,
        string? channel = null,
        bool? isSuccess = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var specification = HasHistory();

        if (notificationId.HasValue)
            specification = specification.And(ByNotificationId(notificationId.Value));

        if (recipientId.HasValue)
            specification = specification.And(ByRecipientId(recipientId.Value));

        if (!string.IsNullOrEmpty(channel))
            specification = specification.And(HasHistoryWithChannel(channel));

        if (isSuccess.HasValue)
            specification = specification.And(HasHistoryWithSuccess(isSuccess.Value));

        if (fromDate.HasValue)
            specification = specification.And(HasHistoryFromDate(fromDate.Value));

        if (toDate.HasValue)
            specification = specification.And(HasHistoryToDate(toDate.Value));

        return specification;
    }
}
