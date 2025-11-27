using Identity.Application.AccountPermissions;
using Identity.Gateways.AccountPermissions.AttachPermissionToAccount;
using Identity.Gateways.AccountPermissions.Shared;
using Identity.Infrastructure.AccountPermissions.EventListeners.OnRemoved;
using Identity.Infrastructure.ACL;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.AccountPermissions.DetachPermissionFromAccount;

public sealed class DetachPermissionFromAccountGateway(
    QueryAccountAndPermission query,
    LoggingOnAccountPermissionRemovedEventListener loggingListener,
    NpgSqlOnAccountPermissionRemovedEventListener databaseListener
) 
    : IGateway<DetachPermissionFromAccountRequest, AccountPermissionResponse>
{
    public async Task<Result<AccountPermissionResponse>> Execute(DetachPermissionFromAccountRequest request)
    {
        AccountPermission? accountPermission = await query.Query(
            request.AccountId, 
            request.PermissionId, 
            request.Ct
        );
        
        if (accountPermission == null) return Error.NotFound("Не найдены учетная запись или разрешение");

        AccountPermissionResponse response = new();
        OnAccountPermissionRemovedPipeLine pipeline = new(
            databaseListener,
            new OnAccountPermissionRemovedPipeLine(loggingListener,
                new OnAccountPermissionRemovedPipeLine(response))
        );
        
        AccountPermission observed = accountPermission.AddListener(pipeline);
        Result<Unit> result = await observed.Delete(request.Ct);
        return result.Map(() => response);
    }
}