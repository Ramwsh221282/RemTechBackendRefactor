namespace Identity.Domain.Contracts.Outbox;

public interface IAccountOutboxMessagePublisher
{
	bool CanPublish(IdentityOutboxMessage message);
	Task Publish(IdentityOutboxMessage message, CancellationToken ct = default);
}
