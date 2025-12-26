using Identity.Contracts.Accounts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class FakeOnAccountActivationRequiredEventListener : IOnAccountActivationRequiredListener
{
    public Task<Result<Unit>> React(AccountData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}