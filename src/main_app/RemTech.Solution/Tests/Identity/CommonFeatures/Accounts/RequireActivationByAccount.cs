using Identity.Gateways.Accounts.RequireActivation;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class RequireActivationByAccount(IServiceProvider sp)
{
    public async Task<Result<RequireActivationResponse>> Invoke(Guid id)
    {
        RequireActivationRequest request = new(id, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RequireActivationRequest, RequireActivationResponse> service = 
            scope.Resolve<IGateway<RequireActivationRequest, RequireActivationResponse>>();
        return await service.Execute(request);
    }
}