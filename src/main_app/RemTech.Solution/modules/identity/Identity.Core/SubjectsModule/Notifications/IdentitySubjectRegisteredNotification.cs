using Identity.Core.SubjectsModule.Models;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Notifications;

public sealed record IdentitySubjectRegisteredNotification(IdentitySubjectSnapshot Snapshot) : IdentitySubjectNotification(Snapshot);