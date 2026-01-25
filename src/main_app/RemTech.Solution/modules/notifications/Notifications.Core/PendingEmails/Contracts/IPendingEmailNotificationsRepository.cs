namespace Notifications.Core.PendingEmails.Contracts;

public interface IPendingEmailNotificationsRepository
{
	public Task Add(PendingEmailNotification notification, CancellationToken ct = default);
	public Task<PendingEmailNotification[]> GetMany(
		PendingEmailNotificationsSpecification spec,
		CancellationToken ct = default
	);
	public Task<int> Remove(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default);
}
