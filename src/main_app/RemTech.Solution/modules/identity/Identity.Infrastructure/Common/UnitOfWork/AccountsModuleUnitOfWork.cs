using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;

namespace Identity.Infrastructure.Common.UnitOfWork;

public sealed class AccountsModuleUnitOfWork(
	AccountsChangeTracker accounts,
	AccountTicketsChangeTracker accountTickets,
	PermissionsChangeTracker permissions,
	IdentityOutboxMessageChangeTracker outboxMessages
) : IAccountsModuleUnitOfWork
{
	private AccountsChangeTracker Accounts { get; } = accounts;
	private AccountTicketsChangeTracker AccountTickets { get; } = accountTickets;
	private PermissionsChangeTracker Permissions { get; } = permissions;
	private IdentityOutboxMessageChangeTracker OutboxMessages { get; } = outboxMessages;

	public Task Save(IEnumerable<Account> accounts, CancellationToken ct = default) =>
		Accounts.SaveChanges(accounts, ct);

	public Task Save(Account account, CancellationToken ct = default) => Accounts.SaveChanges([account], ct);

	public Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default) =>
		Permissions.SaveChanges(permissions, ct);

	public Task Save(Permission permission, CancellationToken ct = default) =>
		Permissions.SaveChanges([permission], ct);

	public Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default) =>
		AccountTickets.SaveChanges(tickets, ct);

	public Task Save(AccountTicket ticket, CancellationToken ct = default) => AccountTickets.SaveChanges([ticket], ct);

	public Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default) =>
		OutboxMessages.Save(messages, ct);

	public Task Save(IdentityOutboxMessage message, CancellationToken ct = default) =>
		OutboxMessages.Save([message], ct);

	public void Track(IEnumerable<Account> accounts) => Accounts.StartTracking(accounts);

	public void Track(IEnumerable<AccountTicket> tickets) => AccountTickets.StartTracking(tickets);

	public void Track(IEnumerable<Permission> permissions) => Permissions.StartTracking(permissions);

	public void Track(IEnumerable<IdentityOutboxMessage> messages) => OutboxMessages.Track(messages);
}
