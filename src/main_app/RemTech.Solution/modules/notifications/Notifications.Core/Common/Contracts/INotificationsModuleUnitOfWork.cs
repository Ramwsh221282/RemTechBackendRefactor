using Notifications.Core.Mailers;
using Notifications.Core.PendingEmails;

namespace Notifications.Core.Common.Contracts;

public interface INotificationsModuleUnitOfWork
{
    public Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default);
    public Task Save(Mailer mailer, CancellationToken ct = default);
    public Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default);

    public void Track(IEnumerable<Mailer> mailers);
    public void Track(IEnumerable<PendingEmailNotification> notifications);
}
