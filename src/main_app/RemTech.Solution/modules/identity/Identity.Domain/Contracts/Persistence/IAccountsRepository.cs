using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccountsRepository
{
    public Task Add(Account account, CancellationToken ct = default);
    public Task<bool> Exists(AccountSpecification specification, CancellationToken ct = default);
    public Task<Result<Account>> Find(AccountSpecification specification, CancellationToken ct = default);
}
