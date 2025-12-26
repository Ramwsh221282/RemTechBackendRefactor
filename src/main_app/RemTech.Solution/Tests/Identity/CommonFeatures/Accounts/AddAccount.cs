using Identity.Gateways.Accounts.AddAccount;
using Identity.Gateways.Accounts.Responses;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class AddAccount(IServiceProvider sp)
{
    public async Task<Result<AccountResponse>> Invoke(string name, string email, string password)
    {
        AddAccountRequest request = new(name, email, password, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AddAccountRequest, AccountResponse> service = scope.Resolve<IGateway<AddAccountRequest, AccountResponse>>();
        return await service.Execute(request);
    }
}