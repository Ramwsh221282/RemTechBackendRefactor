using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence;

public sealed class IdentitySubjectPersistenceTarget : IdentitySubjectNotificationTarget
{
    private readonly IEnumerable<IIdentitySubjectNotificationPersistenceHandler> _handlers;

    public IdentitySubjectPersistenceTarget(IEnumerable<IIdentitySubjectNotificationPersistenceHandler> handlers)
    {
        _handlers = handlers;
    }

    public override void Record(IdentitySubjectNotification notification)
    {
        foreach (IIdentitySubjectNotificationPersistenceHandler handler in _handlers)
            handler.Record(notification);
    }

    public async Task<Result<Unit>> Handle(NpgSqlSession session, CancellationToken ct = default)
    {
        foreach (IIdentitySubjectNotificationPersistenceHandler handler in _handlers)
        {
            Result<Unit> result = await handler.Handle(session, ct);
            if (result.IsFailure)
                return result.Error;
        }

        return Unit.Value;
    }
}