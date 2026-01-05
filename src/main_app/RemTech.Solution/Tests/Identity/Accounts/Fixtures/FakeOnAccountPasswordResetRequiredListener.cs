using Identity.Contracts.Accounts;
using Identity.Domain;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class FakeOnAccountPasswordResetRequiredListener : IOnAccountPasswordResetRequiredListener
{
    public Task<Result<Unit>> React(AccountData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}