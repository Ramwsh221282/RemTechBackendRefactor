namespace Identity.Infrastructure.Outbox;

public sealed record IdentityOutboxMessage(
    Guid Id,
    string Type,
    string Payload,
    DateTime Created,
    bool WasSent);