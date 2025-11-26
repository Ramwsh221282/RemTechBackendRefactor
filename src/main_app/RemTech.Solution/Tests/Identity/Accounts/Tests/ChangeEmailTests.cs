using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Responses;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Accounts.Fixtures;

namespace Tests.Identity.Accounts.Tests;

public sealed class ChangeEmailTests(AccountsTestsFixture fixture) : IClassFixture<AccountsTestsFixture>
{
    private readonly AccountFeatureFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Change_Email_Test_Success()
    {
        const string defaultEmail = "someEmail@mail.com";
        const string name = "testName";
        const string password = "TestPassword";
        const string otherEmail = "otherEmail@mail.com";
        Result<AccountResponse> result = await _facade.AddAccount(name, defaultEmail, password);
        Assert.True(result.IsSuccess);
        Guid id = result.Value.Id;
        await _facade.MakeAccountActivated(id);
        Result<AccountResponse> changing = await _facade.ChangeEmail(id, otherEmail);
        Assert.True(changing.IsSuccess);
        Result<AccountData> accountData = await _facade.GetAccount(id);
        Assert.True(accountData.IsSuccess);
        Assert.Equal(otherEmail, accountData.Value.Email);
        Assert.NotEqual(defaultEmail, accountData.Value.Email);
    }
    
    [Fact]
    private async Task Change_Email_Account_Not_Found()
    {
        const string otherEmail = "otherEmail@mail.com";
        Result<AccountResponse> changing = await _facade.ChangeEmail(Guid.NewGuid(), otherEmail);
        Assert.True(changing.IsFailure);
    }
    
    [Fact]
    private async Task Change_Email_Account_Not_Activated_Failure()
    {
        const string defaultEmail = "someEmail@mail.com";
        const string name = "testName";
        const string password = "TestPassword";
        const string otherEmail = "otherEmail@mail.com";
        Result<AccountResponse> result = await _facade.AddAccount(name, defaultEmail, password);
        Guid id = result.Value.Id;
        Result<AccountResponse> changing = await _facade.ChangeEmail(id, otherEmail);
        Assert.True(changing.IsFailure);
    }
    
    [Fact]
    private async Task Change_Email_Invalid_Email_Failure()
    {
        const string defaultEmail = "someEmail@mail.com";
        const string name = "testName";
        const string password = "TestPassword";
        const string otherEmail = "   ";
        Result<AccountResponse> result = await _facade.AddAccount(name, defaultEmail, password);
        Assert.True(result.IsSuccess);
        Guid id = result.Value.Id;
        await _facade.MakeAccountActivated(id);
        Result<AccountResponse> changing = await _facade.ChangeEmail(id, otherEmail);
        Assert.True(changing.IsFailure);
    }
}
