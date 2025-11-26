using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Contracts.Permissions.Defaults;

public sealed class NoneOnPermissionRenamedEventListener : IOnPermissionRenamedEventListener
{
    public Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        return Task.FromResult(Result.Success(Unit.Value));
    }
}