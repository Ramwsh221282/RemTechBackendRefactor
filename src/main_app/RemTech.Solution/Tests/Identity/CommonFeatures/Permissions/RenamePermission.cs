using Identity.Gateways.Permissions.RenamePermission;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Permissions;

public sealed class RenamePermission(IServiceProvider sp)
{
    public async Task<Result<RenamePermissionResponse>> Invoke(Guid permissionId, string name)
    {
        CancellationToken ct = CancellationToken.None;
        RenamePermissionRequest request = new(permissionId, name, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RenamePermissionRequest, RenamePermissionResponse> service =
            scope.Resolve<IGateway<RenamePermissionRequest, RenamePermissionResponse>>();
        return await service.Execute(request);
    }
}