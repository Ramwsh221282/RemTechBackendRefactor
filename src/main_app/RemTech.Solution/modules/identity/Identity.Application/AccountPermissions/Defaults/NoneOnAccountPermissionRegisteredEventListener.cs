using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.AccountPermissions.Defaults;

public sealed class NoneOnAccountPermissionRegisteredEventListener : IOnAccountPermissionCreatedEventListener
{
    public Task<Result<Unit>> React(
        AccountPermissionData data, 
        CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}