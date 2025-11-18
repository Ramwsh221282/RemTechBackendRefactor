namespace Identity.Outbox;

public sealed record IdentityOutboxMessage(
    Guid Id,
    string Type,
    string Body,
    string Exchange,
    string RoutingKey,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    int RetryCount);