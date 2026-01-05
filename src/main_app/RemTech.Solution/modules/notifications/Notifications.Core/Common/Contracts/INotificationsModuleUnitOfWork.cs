using Notifications.Core.Mailers;

namespace Notifications.Core.Common.Contracts;

public interface INotificationsModuleUnitOfWork
{
    Task Save(IEnumerable<Mailer> mailers, CancellationToken ct = default);
    Task Save(Mailer mailer, CancellationToken ct = default);
    void Track(IEnumerable<Mailer> mailers);
}