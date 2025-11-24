namespace RemTech.SharedKernel.Infrastructure.Outbox;

public sealed record OutboxMessage(
    Guid Id,
    string Type,
    string Body,
    string Queue,
    string Exchange,
    string RoutingKey,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    int RetryCount)
{
    public static OutboxMessage New(string queue, string exchange, string routingKey, string type, string body)
    {
        return new OutboxMessage(
            Guid.NewGuid(),
            type,
            body,
            queue,
            exchange,
            routingKey,
            DateTime.UtcNow,
            null,
            0
        );
    }
}