using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Domain.Subjects.Events;

public record SubjectAuthorized(SubjectSnapshot Snapshot) : Notification;