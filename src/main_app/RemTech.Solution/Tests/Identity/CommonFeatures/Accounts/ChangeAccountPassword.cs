using Identity.Gateways.Accounts.ChangePassword;
using Identity.Gateways.Accounts.Responses;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class ChangeAccountPassword(IServiceProvider sp)
{
    public async Task<Result<AccountResponse>> Invoke(Guid id, string password)
    {
        ChangeAccountPasswordRequest request = new(id, password, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ChangeAccountPasswordRequest, AccountResponse> service =
            scope.Resolve<IGateway<ChangeAccountPasswordRequest, AccountResponse>>();
        return await service.Execute(request);
    }
}