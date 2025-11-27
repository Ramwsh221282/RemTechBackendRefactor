using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.AccountPermissions.EventListeners.OnRemoved;

public sealed class NpgSqlOnAccountPermissionRemovedEventListener(
    NpgSqlAccountPermissionsStorage storage) :
    IOnAccountPermissionRemovedEventListener
{
    public async Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        await storage.Remove(new AccountPermission(data), ct);
        return Unit.Value;
    }
}