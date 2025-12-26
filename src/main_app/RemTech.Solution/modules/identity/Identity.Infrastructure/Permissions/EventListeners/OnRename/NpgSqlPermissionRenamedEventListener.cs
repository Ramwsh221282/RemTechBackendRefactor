using Identity.Application.Permissions;
using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Infrastructure.Permissions.EventListeners.OnRename;

public sealed class NpgSqlPermissionRenamedEventListener(
    NpgSqlPermissionsStorage storage
    ) : 
    IOnPermissionRenamedEventListener
{
    public async Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default)
    {
        Permission? withName = await storage.Fetch(new PermissionQueryArgs(Name: data.Name), ct);
        if (withName != null) return Error.Conflict($"Разрешение с названием: {data.Name} уже существует");
        await storage.Update(new Permission(data), ct);
        return Unit.Value;
    }
}