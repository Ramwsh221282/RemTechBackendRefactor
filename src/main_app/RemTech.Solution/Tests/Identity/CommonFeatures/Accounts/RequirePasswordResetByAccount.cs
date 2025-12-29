using Identity.Gateways.Accounts.RequirePasswordReset;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class RequirePasswordResetByAccount(IServiceProvider sp)
{
    public async Task<Result<RequirePasswordResetResponse>> Invoke(Guid id)
    {
        RequirePasswordResetRequest request = new(id, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse> service =
            scope.Resolve<IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse>>();
        return await service.Execute(request);
    }
}