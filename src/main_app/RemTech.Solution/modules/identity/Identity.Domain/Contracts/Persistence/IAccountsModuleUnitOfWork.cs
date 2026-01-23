using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccountsModuleUnitOfWork
{
	public Task Save(IEnumerable<Account> accounts, CancellationToken ct = default);
	public Task Save(Account account, CancellationToken ct = default);
	public Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default);
	public Task Save(Permission permission, CancellationToken ct = default);
	public Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default);
	public Task Save(AccountTicket ticket, CancellationToken ct = default);
	public Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default);
	public Task Save(IdentityOutboxMessage message, CancellationToken ct = default);
	public void Track(IEnumerable<Account> accounts);
	public void Track(IEnumerable<AccountTicket> tickets);
	public void Track(IEnumerable<Permission> permissions);
	public void Track(IEnumerable<IdentityOutboxMessage> messages);
}
