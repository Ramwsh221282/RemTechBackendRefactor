using Identity.Application.AccountPermissions;
using Identity.Gateways.AccountPermissions.Shared;
using Identity.Infrastructure.AccountPermissions.EventListeners.OnRegistered;
using Identity.Infrastructure.ACL;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.AccountPermissions.AttachPermissionToAccount;

public sealed class AttachPermissionToAccountGateway(
    QueryAccountAndPermission query,
    LoggingOnRegisteredAccountPermissionEventListener loggingListener,
    NpgSqlOnAccountPermissionRegisteredEventListener databaseListener
) 
    : IGateway<AttachPermissionToAccountRequest, AccountPermissionResponse>
{
    public async Task<Result<AccountPermissionResponse>> Execute(AttachPermissionToAccountRequest request)
    {
        AccountPermissionResponse response = new();
        
        OnAccountPermissionRegisteredEventListenersPipeLine pipeline = new(
            databaseListener,
            new OnAccountPermissionRegisteredEventListenersPipeLine(
                loggingListener,
                new OnAccountPermissionRegisteredEventListenersPipeLine(
                    response
                )));
        
        AccountPermission? accountPermission = await query.Query(request.AccountId, request.PermissionId);
        if (accountPermission is null) return Error.NotFound("Не найдены учетная запись или разрешение.");
        AccountPermission observed = accountPermission.AddListener(pipeline);
        Result<Unit> result = await observed.Save(request.Ct);
        return result.Map(() => response);
    }
}