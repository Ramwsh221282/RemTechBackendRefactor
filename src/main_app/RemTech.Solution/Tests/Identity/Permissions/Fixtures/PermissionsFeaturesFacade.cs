using Identity.Application.Permissions;
using Identity.Contracts.Permissions;
using Identity.Gateways.Permissions.AddPermission;
using Identity.Gateways.Permissions.RenamePermission;
using Identity.Infrastructure.Permissions;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.Permissions.Fixtures;

public sealed class PermissionsFeaturesFacade(IServiceProvider sp)
{
    public async Task<Result<AddPermissionResponse>> AddPermission(string name)
    {
        CancellationToken ct = CancellationToken.None;
        AddPermissionRequest request = new(name, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AddPermissionRequest, AddPermissionResponse> gateway = 
            scope.Resolve<IGateway<AddPermissionRequest, AddPermissionResponse>>();
        return await gateway.Execute(request);
    }

    public async Task<Permission?> Get(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        NpgSqlPermissionsStorage permissions = scope.Resolve<NpgSqlPermissionsStorage>();
        PermissionQueryArgs args = new(Id: id);
        Permission? permission = await permissions.Fetch(args, CancellationToken.None);
        return permission;
    }
    
    public async Task<Result<RenamePermissionResponse>> RenamePermission(
        Guid id,
        string name
    )
    {
        CancellationToken ct = CancellationToken.None;
        RenamePermissionRequest request = new(id, name, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RenamePermissionRequest, RenamePermissionResponse> gateway =
            scope.Resolve<IGateway<RenamePermissionRequest, RenamePermissionResponse>>();
        return await gateway.Execute(request);
    }
}