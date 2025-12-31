using Identity.Gateways.AccountPermissions.DetachPermissionFromAccount;
using Identity.Gateways.AccountPermissions.Shared;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.AccountPermissions;

public sealed class DetachPermissionFromAccount(IServiceProvider sp)
{
    public async Task<Result<AccountPermissionResponse>> Invoke(Guid accountId, Guid permissionId)
    {
        CancellationToken ct = CancellationToken.None;
        DetachPermissionFromAccountRequest request = new(accountId, permissionId, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<DetachPermissionFromAccountRequest, AccountPermissionResponse> service =
            scope.Resolve<IGateway<DetachPermissionFromAccountRequest, AccountPermissionResponse>>();
        return await service.Execute(request);
    }
}