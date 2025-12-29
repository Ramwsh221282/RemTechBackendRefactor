using Identity.Gateways.Permissions.AddPermission;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Permissions;

public sealed class AddPermission(IServiceProvider sp)
{
    public async Task<Result<AddPermissionResponse>> Invoke(string name)
    {
        CancellationToken ct = CancellationToken.None;
        AddPermissionRequest request = new(name, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AddPermissionRequest, AddPermissionResponse> service =
            scope.Resolve<IGateway<AddPermissionRequest, AddPermissionResponse>>();
        return await service.Execute(request);
    }
}