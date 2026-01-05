namespace Identity.Domain.Contracts.Outbox;

public sealed class OutboxMessagesRegistry(IEnumerable<IAccountOutboxMessagePublisher> publishers)
{
    public async Task Publish(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default)
    {
        foreach (IdentityOutboxMessage message in messages)
        {
            foreach (IAccountOutboxMessagePublisher publisher in publishers)
            {
                if (publisher.CanPublish(message))
                    await publisher.Publish(message, ct);
            }
        }
    }
}