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

	public async Task Save(IEnumerable<Account> account, CancellationToken ct = default) =>
		await Accounts.SaveChanges(account, ct);

	public async Task Save(Account account, CancellationToken ct = default) =>
		await Accounts.SaveChanges([account], ct);

	public async Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default) =>
		await Permissions.SaveChanges(permissions, ct);

	public async Task Save(Permission permission, CancellationToken ct = default) =>
		await Permissions.SaveChanges([permission], ct);

	public async Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default) =>
		await AccountTickets.SaveChanges(tickets, ct);

	public async Task Save(AccountTicket ticket, CancellationToken ct = default) =>
		await AccountTickets.SaveChanges([ticket], ct);

	public async Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default) =>
		await OutboxMessages.Save(messages, ct);

	public async Task Save(IdentityOutboxMessage message, CancellationToken ct = default) =>
		await OutboxMessages.Save([message], ct);

	public void Track(IEnumerable<Account> accounts) => Accounts.StartTracking(accounts);

	public void Track(IEnumerable<AccountTicket> tickets) => AccountTickets.StartTracking(tickets);

	public void Track(IEnumerable<Permission> permissions) => Permissions.StartTracking(permissions);

	public void Track(IEnumerable<IdentityOutboxMessage> messages) => OutboxMessages.Track(messages);
}
