using Identity.Domain.Accounts.Models;
using Identity.Domain.Permissions;
using Identity.Domain.Tickets;

namespace Identity.Domain.Contracts;

public interface IAccountsModuleUnitOfWork
{
    Task Save(IEnumerable<Account> account, CancellationToken ct = default);
    Task Save(Account account, CancellationToken ct = default);
    Task Save(IEnumerable<Permission> permissions, CancellationToken ct = default);
    Task Save(Permission permission, CancellationToken ct = default);
    Task Save(IEnumerable<AccountTicket> tickets, CancellationToken ct = default);
    Task Save(AccountTicket ticket, CancellationToken ct = default);
    void Track(IEnumerable<Account> accounts);
    void Track(IEnumerable<AccountTicket> tickets);
    void Track(IEnumerable<Permission> permissions);
}