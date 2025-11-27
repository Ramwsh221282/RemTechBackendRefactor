using Identity.Application.AccountPermissions;
using Identity.Contracts.AccountPermissions;
using Identity.Contracts.AccountPermissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.AccountPermissions.EventListeners.OnRegistered;

public sealed class NpgSqlOnAccountPermissionRegisteredEventListener(
    NpgSqlAccountPermissionsStorage storage
) :
    IOnAccountPermissionCreatedEventListener
{
    public async Task<Result<Unit>> React(AccountPermissionData data, CancellationToken ct = default)
    {
        AccountPermissionQueryArgs args = new(AccountId: data.AccountId, PermissionId: data.PermissionId);
        IAccountPermission? existing = await storage.Fetch(args, ct);
        if (existing != null) return Error.Conflict("У учетной записи уже есть это разрешение.");
        await storage.Persist(new AccountPermission(data), ct);
        return Unit.Value;
    }
}