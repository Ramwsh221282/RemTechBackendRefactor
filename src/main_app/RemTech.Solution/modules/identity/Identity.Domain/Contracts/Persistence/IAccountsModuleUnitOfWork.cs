using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccountsModuleUnitOfWork
{
    Task Save(IEnumerable<Account> account, CancellationToken ct = default);
    Task Save(Account account, CancellationToken ct = default);
    Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default);
    Task Save(Permission permission, CancellationToken ct = default);
    Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default);
    Task Save(AccountTicket ticket, CancellationToken ct = default);
    Task Save(IEnumerable<IdentityOutboxMessage> messages, CancellationToken ct = default);
    Task Save(IdentityOutboxMessage message, CancellationToken ct = default);
    void Track(IEnumerable<Account> accounts);
    void Track(IEnumerable<AccountTicket> tickets);
    void Track(IEnumerable<Permission> permissions);
    void Track(IEnumerable<IdentityOutboxMessage> messages);
}