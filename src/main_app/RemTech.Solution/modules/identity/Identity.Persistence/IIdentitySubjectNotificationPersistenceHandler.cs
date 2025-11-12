using Identity.Core.SubjectsModule.Notifications.Abstractions;
using RemTech.Functional.Extensions;
using RemTech.NpgSql.Abstractions;

namespace Identity.Persistence;

public interface IIdentitySubjectNotificationPersistenceHandler
{
    void Record(IdentitySubjectNotification notification);
    Task<Result<Unit>> Handle(NpgSqlSession session, CancellationToken ct = default);
}