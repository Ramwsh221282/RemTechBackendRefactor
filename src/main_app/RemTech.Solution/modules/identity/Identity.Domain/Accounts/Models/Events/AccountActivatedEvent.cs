using RemTech.SharedKernel.Core.DomainEvents;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;

namespace Identity.Domain.Accounts.Models.Events;

public sealed class AccountActivatedEvent(Guid accountId, string accountEmail, string accountLogin) : IDomainEvent
{
	public AccountActivatedEvent(Account account)
		: this(account.Id.Value, account.Email.Value, account.Login.Value) { }

	public Guid AccountId { get; } = accountId;
	public string AccountEmail { get; } = accountEmail;
	public string AccountLogin { get; } = accountLogin;

	public async Task PublishTo(IDomainEventHandler handler, CancellationToken ct = default) =>
		await handler.Handle(this, ct);
}
