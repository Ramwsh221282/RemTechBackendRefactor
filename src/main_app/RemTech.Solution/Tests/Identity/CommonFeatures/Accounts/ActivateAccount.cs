using Identity.Gateways.Accounts.Activate;
using Identity.Gateways.Accounts.Responses;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class ActivateAccount(IServiceProvider sp)
{
    public async Task<Result<AccountResponse>> Invoke(Guid id)
    {
        ActivateAccountRequest request = new(id, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ActivateAccountRequest, AccountResponse> service =
            scope.Resolve<IGateway<ActivateAccountRequest, AccountResponse>>();
        return await service.Execute(request);
    }
}