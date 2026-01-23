using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Contracts.Persistence;

public interface IAccountsRepository
{
	Task Add(Account account, CancellationToken ct = default);
	Task<bool> Exists(AccountSpecification specification, CancellationToken ct = default);
	Task<Result<Account>> Find(AccountSpecification specification, CancellationToken ct = default);
}
