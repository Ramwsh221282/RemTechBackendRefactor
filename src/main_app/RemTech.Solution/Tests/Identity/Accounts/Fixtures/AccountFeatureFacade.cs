using Dapper;
using Identity.Application.Accounts;
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
using RemTech.SharedKernel.Infrastructure.NpgSql;

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

    public async Task<Result<IAccountData>> GetAccount(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IAccountPersister persister = scope.Resolve<IAccountPersister>();
        Result<IAccount> account = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: id),
            CancellationToken.None
        );
        
        if (account.IsFailure) return account.Error;
        IAccountRepresentation representation = account.Value.Represent(AccountRepresentation.Empty());
        return Result.Success(representation.Data);
    }

    public async Task<bool> IsEncryptedPasswordEqualToPlain(Guid accountId, string inputPassword)
    {
        CancellationToken ct = CancellationToken.None;
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IAccountPersister persister = scope.Resolve<IAccountPersister>();
        IAccountDecrypter decrypted = scope.Resolve<IAccountDecrypter>();
        Result<IAccount> account = await PersistingAccount.Get(
            persister,
            new AccountQueryArgs(Id: accountId),
            ct
        );
        if (account.IsFailure) throw new InvalidOperationException($"Account with ID: {accountId} does not exist");
        IAccount withDecryptedData = await decrypted.Decrypt(account.Value, ct);
        string decryptedPassword = withDecryptedData.Represent(AccountRepresentation.Empty()).Data.Password;
        return decryptedPassword == inputPassword;
    }
    
    public async Task MakeAccountActivated(Guid id)
    {
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        await using NpgSqlSession session = scope.Resolve<NpgSqlSession>();
        const string sql = "UPDATE identity_module.accounts SET activated = TRUE where id = @id";
        CommandDefinition command = new(sql, new { id });
        await session.Execute(command);
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