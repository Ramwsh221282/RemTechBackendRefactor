using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Activate;
using Identity.Gateways.Accounts.AddAccount;
using Identity.Gateways.Accounts.ChangeEmail;
using Identity.Gateways.Accounts.ChangePassword;
using Identity.Gateways.Accounts.RequireActivation;
using Identity.Gateways.Accounts.RequirePasswordReset;
using Identity.Gateways.Accounts.Responses;
using Identity.Infrastructure.Accounts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Configuration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Identity.Accounts.Fixtures;

public sealed class AccountFeatureFacade(IServiceProvider sp)
{
    public async Task<Result<AccountResponse>> AddAccount(string name, string email, string password)
    {
        AddAccountRequest request = new(name, email, password, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AddAccountRequest, AccountResponse> service = scope.Resolve<IGateway<AddAccountRequest, AccountResponse>>();
        return await service.Execute(request);
    }

    public async Task<bool> AccountCreated(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IAccountPersister persister = scope.Resolve<IAccountPersister>();
        Result<IAccount> account = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: id),
            CancellationToken.None
        );
        return account.IsSuccess;
    }
    
    public async Task<Result<AccountResponse>> ChangeEmail(Guid id, string email)
    {
        ChangeAccountEmailRequest request = new(id, email, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ChangeAccountEmailRequest, AccountResponse> service =
            scope.Resolve<IGateway<ChangeAccountEmailRequest, AccountResponse>>();
        return await service.Execute(request);
    }
    
    public async Task<Result<AccountResponse>> ChangePassword(Guid id, string password)
    {
        ChangeAccountPasswordRequest request = new(id, password, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ChangeAccountPasswordRequest, AccountResponse> service =
            scope.Resolve<IGateway<ChangeAccountPasswordRequest, AccountResponse>>();
        return await service.Execute(request);
    }
    
    public async Task<Result<AccountResponse>> Activate(Guid id)
    {
        ActivateAccountRequest request = new(id, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<ActivateAccountRequest, AccountResponse> service =
            scope.Resolve<IGateway<ActivateAccountRequest, AccountResponse>>();
        return await service.Execute(request);
    }
    
    public async Task<Result<RequireActivationResponse>> RequireActivation(Guid id)
    {
        RequireActivationRequest request = new(id, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RequireActivationRequest, RequireActivationResponse> service = 
            scope.Resolve<IGateway<RequireActivationRequest, RequireActivationResponse>>();
        return await service.Execute(request);
    }
    
    public async Task<Result<RequirePasswordResetResponse>> RequirePasswordReset(Guid id)
    {
        RequirePasswordResetRequest request = new(id, CancellationToken.None);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse> service =
            scope.Resolve<IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse>>();
        return await service.Execute(request);
    }
}