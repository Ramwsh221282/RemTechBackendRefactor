using RemTech.Outbox.Shared;

namespace Identity.Outbox;

public interface IHowToProcessIdentityOutboxMessage
{
    Task ProcessMessage(ProcessedOutboxMessages messages, OutboxMessage message, CancellationToken ct = default);
}