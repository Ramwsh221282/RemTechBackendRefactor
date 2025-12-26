using Identity.Gateways.Accounts.Responses;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Accounts.Fixtures;

namespace Tests.Identity.Accounts.Tests;

public sealed class ActivateAccountTests(AccountsTestsFixture fixture) : IClassFixture<AccountsTestsFixture>
{
    private readonly AccountFeatureFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Activate_Account_Success()
    {
        const string name = "someName";
        const string email = "someEmail@mail.com";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount.Invoke(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        Result<AccountResponse> activation = await _facade.ActivateAccount.Invoke(accountId);
        Assert.True(activation.IsSuccess);
    }

    [Fact]
    private async Task Activate_Already_Activated_Account_Failure()
    {
        const string name = "someName";
        const string email = "someEmail@mail.com";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount.Invoke(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.ActivateAccount.Invoke(accountId);
        Result<AccountResponse> activation = await _facade.ActivateAccount.Invoke(accountId);
        Assert.True(activation.IsFailure);
    }
    
    [Fact]
    private async Task Activate_Not_Existing_Account_Failure()
    {
        Result<AccountResponse> activation = await _facade.ActivateAccount.Invoke(Guid.NewGuid());
        Assert.True(activation.IsFailure);
    }
}