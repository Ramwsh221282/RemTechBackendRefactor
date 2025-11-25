namespace Identity.Core.Accounts.Events;

public sealed record AccountPasswordChangedEvent(
    Guid Id,
    string NewPassword
) : Event;