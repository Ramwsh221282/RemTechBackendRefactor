namespace Identity.Notifier.Port;

public sealed record UserNotification(
    Guid TokenId,
    string UserEmail,
    string Subject,
    string Message
);
