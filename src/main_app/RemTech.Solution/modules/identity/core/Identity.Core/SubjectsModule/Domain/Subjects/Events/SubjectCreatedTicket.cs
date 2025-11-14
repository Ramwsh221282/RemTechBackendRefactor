using Identity.Core.SubjectsModule.Notifications.Abstractions;

namespace Identity.Core.SubjectsModule.Domain.Subjects.Events;

public sealed record SubjectCreatedTicket(Guid Id, Guid CreatorId, string Type) : Notification;