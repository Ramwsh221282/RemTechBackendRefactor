using Identity.Gateways.AccountPermissions.AttachPermissionToAccount;
using Identity.Gateways.AccountPermissions.Shared;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.AccountPermissions;

public sealed class AttachPermissionForAccount(IServiceProvider sp)
{
    public async Task<Result<AccountPermissionResponse>> Invoke(Guid accountId, Guid permissionId)
    {
        CancellationToken ct = CancellationToken.None;
        AttachPermissionToAccountRequest request = new(accountId, permissionId, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AttachPermissionToAccountRequest, AccountPermissionResponse> service =
            scope.ServiceProvider.GetRequiredService<IGateway<AttachPermissionToAccountRequest, AccountPermissionResponse>>();
        return await service.Execute(request);
    }
}