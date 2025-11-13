using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Modules;
using Identity.Core.SubjectsModule.Notifications;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Persistence.NpgSql.SubjectsModule;

public static class NpgSqlIdentitySubjectsModule
{
    public static AsyncNotificationHandle<Registered> OnRegistered(
        NpgSqlSubjectCommands commands)
    {
        return async (@event, ct) => await commands.Insert(Subject.Create(@event.Snapshot), ct);
    }
}