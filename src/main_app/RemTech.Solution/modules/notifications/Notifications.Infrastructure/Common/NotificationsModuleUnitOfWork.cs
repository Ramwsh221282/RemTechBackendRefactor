using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers;
using Notifications.Infrastructure.Mailers;

namespace Notifications.Infrastructure.Common;

public sealed class NotificationsModuleUnitOfWork(
    MailersChangeTracker mailers
    ) : INotificationsModuleUnitOfWork
{
    private MailersChangeTracker Mailers { get; } = mailers;
    
    public async Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default) =>
        await Mailers.Save(mailers, ct);

    public async Task Save(Mailer mailer, CancellationToken ct = default) =>
        await Mailers.Save([mailer], ct);

    public void Track(IEnumerable<Mailer> mailers) => 
        Mailers.Track(mailers);
}