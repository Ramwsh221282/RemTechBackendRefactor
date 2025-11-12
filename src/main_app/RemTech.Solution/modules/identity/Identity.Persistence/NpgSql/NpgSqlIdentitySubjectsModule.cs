using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Persistence.NpgSql;

public static class NpgSqlIdentitySubjectsModule
{
    public static AsyncSubjectNotificationHandle<IdentitySubjectRegisteredNotification> OnRegistered(
        NpgSqlIdentitySubjectCommands commands)
    {
        return async (@event, ct) => await commands.Insert(Subject.Create(@event.Snapshot), ct);
    }
}