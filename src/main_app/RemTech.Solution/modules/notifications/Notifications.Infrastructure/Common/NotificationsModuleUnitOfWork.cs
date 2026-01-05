using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers;
using Notifications.Core.PendingEmails;
using Notifications.Infrastructure.Mailers;
using Notifications.Infrastructure.PendingEmails;

namespace Notifications.Infrastructure.Common;

public sealed class NotificationsModuleUnitOfWork(
    MailersChangeTracker mailers,
    PendingEmailsChangeTracker pendingEmails
    ) : INotificationsModuleUnitOfWork
{
    private MailersChangeTracker Mailers { get; } = mailers;
    
    public async Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default) =>
        await Mailers.Save(mailers, ct);

    public async Task Save(Mailer mailer, CancellationToken ct = default) =>
        await Mailers.Save([mailer], ct);

    public async Task Save(IEnumerable<PendingEmailNotification> notifications, CancellationToken ct = default) =>
        await pendingEmails.Save(notifications, ct);

    public void Track(IEnumerable<Mailer> mailers) => 
        Mailers.Track(mailers);

    public void Track(IEnumerable<PendingEmailNotification> notifications) =>
        pendingEmails.Track(notifications);
}