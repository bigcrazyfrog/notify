using Microsoft.EntityFrameworkCore;
using NotifySystem.Core.Domain.Enums;

namespace NotifySystem.Core.Domain.ValueObjects;

[Owned]
public record ContactInfo(string? Email, string? PhoneNumber, string? DeviceToken)
{
    public string GetContact(NotificationType type)
    {
        return type switch
        {
            NotificationType.Email => Email 
                                      ?? throw new Exception("Recipient has no email"),
            NotificationType.SMS => PhoneNumber 
                                    ?? throw new Exception("Recipient has no phone number"),
            NotificationType.Push => DeviceToken
                                     ?? throw new Exception("Recipient has no device token"),
            _ => throw new Exception("Unsupported notification type")
        };
    }

    public bool HasAnyContact()
        => Email != null || PhoneNumber != null || DeviceToken != null;
}