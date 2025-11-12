using Identity.Core.SubjectsModule.Models;
using RemTech.Functional.Extensions;

namespace Identity.Core.SubjectsModule.Notifications.Abstractions;

public abstract record IdentitySubjectNotification(IdentitySubjectSnapshot Snapshot);

public delegate Task<Result<Unit>> AsyncSubjectNotificationHandle<in TEvent>(
    TEvent @event, 
    CancellationToken ct)
    where TEvent : IdentitySubjectNotification;