using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Notifications.Abstractions;

public abstract record IdentitySubjectNotification;

public delegate Task<Result<Unit>> AsyncSubjectNotificationHandle<in TEvent>(
    TEvent @event, 
    CancellationToken ct)
    where TEvent : IdentitySubjectNotification;