namespace Identity.Core.SubjectsModule.Notifications.Abstractions;

public abstract record Notification;

public delegate Task<Result<Unit>> AsyncNotificationHandle<in TEvent>(
TEvent @event, 
    CancellationToken ct)
    where TEvent : Notification;