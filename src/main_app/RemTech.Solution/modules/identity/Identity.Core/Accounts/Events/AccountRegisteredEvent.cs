namespace Identity.Core.Accounts.Events;

public sealed record AccountRegisteredEvent(
    Guid Id,
    string Name,
    string Email,
    string Password,
    bool Activated) : Event;