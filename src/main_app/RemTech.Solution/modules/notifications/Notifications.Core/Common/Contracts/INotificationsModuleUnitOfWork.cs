using Notifications.Core.Mailers;
using Notifications.Core.PendingEmails;

namespace Notifications.Core.Common.Contracts;

public interface INotificationsModuleUnitOfWork
{
    Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default);
    Task Save(Mailer mailer, CancellationToken ct = default);
    Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default);
    
    void Track(IEnumerable<Mailer> mailers);
    void Track(IEnumerable<PendingEmailNotification> notifications);
}