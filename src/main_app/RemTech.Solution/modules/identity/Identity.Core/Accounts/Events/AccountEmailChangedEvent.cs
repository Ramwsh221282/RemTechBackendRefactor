namespace Identity.Core.Accounts.Events;

public sealed record AccountEmailChangedEvent(
    Guid Id,
    string OldEmail,
    string NewEmail
) : Event;