using Identity.Application.Permissions;
using Identity.Contracts.Permissions;
using Identity.Infrastructure.Permissions.EventListeners.OnCreate;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Permissions.AddPermission;

public sealed class AddPermissionGateway(
    NpgSqlPermissionCreatedEventListener databaseListener,
    LoggingPermissionCreatedEventListener loggingListener
) : IGateway<AddPermissionRequest, AddPermissionResponse>
{
    public async Task<Result<AddPermissionResponse>> Execute(AddPermissionRequest request)
    {
        AddPermissionResponse response = new();
        
        OnPermissionCreatedEventListenersPipeline pipeline = 
            new OnPermissionCreatedEventListenersPipeline()
                .Add(databaseListener)
                .Add(loggingListener)
                .Add(response);
        
        PermissionData data = new(Id: Guid.NewGuid(), Name: request.Name);
        Result<Unit> result = await new Permission(data).Register(request.Ct);
        return result.IsFailure ? result.Error : response;
    }
}