namespace Identity.Domain.Contracts.Outbox;

public interface IAccountOutboxMessagePublisher
{
	public bool CanPublish(IdentityOutboxMessage message);
	public Task Publish(IdentityOutboxMessage message, CancellationToken ct = default);
}
