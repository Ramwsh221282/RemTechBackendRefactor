using Identity.Core.SubjectsModule.Domain.Subjects;
using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Notifications;

public sealed record Registered(SubjectSnapshot Snapshot) : Notification;