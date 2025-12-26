using Identity.Gateways.Accounts.ChangeEmail;
using Identity.Gateways.Accounts.Responses;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.CommonFeatures.Accounts;

public sealed class ChangeAccountEmail(IServiceProvider sp)
{
    public async Task<Result<AccountResponse>> Invoke(Guid id, string email)
    {
        ChangeAccountEmailRequest request = new(id, email, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ChangeAccountEmailRequest, AccountResponse> service =
            scope.Resolve<IGateway<ChangeAccountEmailRequest, AccountResponse>>();
        return await service.Execute(request);
    }
}