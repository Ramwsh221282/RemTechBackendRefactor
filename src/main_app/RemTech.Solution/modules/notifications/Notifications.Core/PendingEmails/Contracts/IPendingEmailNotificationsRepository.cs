namespace Notifications.Core.PendingEmails.Contracts;

public interface IPendingEmailNotificationsRepository
{
    Task Add(PendingEmailNotification notification, CancellationToken ct = default);
    Task<PendingEmailNotification[]> GetMany(PendingEmailNotificationsSpecification spec, CancellationToken ct = default);
    Task<int> Remove(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default);
}