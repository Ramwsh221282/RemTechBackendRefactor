using Identity.Domain.Accounts.Models;
using Identity.Domain.Tokens;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccountsRepository
{
    Task Add(Account account, CancellationToken ct = default);
    Task<bool> Exists(AccountSpecification specification, CancellationToken ct = default);
    Task<Result<Account>> Get(AccountSpecification specification, CancellationToken ct = default);
}
