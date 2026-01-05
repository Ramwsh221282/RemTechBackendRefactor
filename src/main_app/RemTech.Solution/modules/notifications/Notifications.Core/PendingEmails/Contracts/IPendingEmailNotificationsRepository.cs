namespace Notifications.Core.PendingEmails.Contracts;

public interface IPendingEmailNotificationsRepository
{
    Task Add(PendingEmailNotification notification, CancellationToken ct = default);
    Task<PendingEmailNotification[]> GetMany(PendingEmailNotificationsSpecification spec, CancellationToken ct = default);
}