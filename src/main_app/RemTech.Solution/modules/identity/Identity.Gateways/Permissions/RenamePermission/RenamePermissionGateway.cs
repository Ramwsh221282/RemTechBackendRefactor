using Identity.Application.Permissions;
using Identity.Application.Permissions.Contracts;
using Identity.Contracts.Permissions;
using Identity.Infrastructure.Permissions.EventListeners.OnRename;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.RenamePermission;

public sealed class RenamePermissionGateway(
    IPermissionsStorage storage,
    NpgSqlPermissionRenamedEventListener databaseListener,
    LoggingPermissionRenamedEventListener loggingListener
) 
    : IGateway<RenamePermissionRequest, RenamePermissionResponse>
{
    public async Task<Result<RenamePermissionResponse>> Execute(RenamePermissionRequest request)
    {
        Permission? permission = await storage.Fetch(new PermissionQueryArgs(Id: request.Id, WithLock: true));
        if (permission == null) return Error.NotFound("Разрешение не найдено");
        
        RenamePermissionResponse response = new();
        OnPermissionRenamedEventListenerPipeLine pipeline =
            new OnPermissionRenamedEventListenerPipeLine()
                .Add(databaseListener)
                .Add(loggingListener);

        Result<Permission> renamed = await permission.AddListener(pipeline).Rename(request.NewName, request.Ct);
        if (renamed.IsFailure) return renamed.Error;
        renamed.Value.Write(response.Add, _ => { });
        return response;
    }
}