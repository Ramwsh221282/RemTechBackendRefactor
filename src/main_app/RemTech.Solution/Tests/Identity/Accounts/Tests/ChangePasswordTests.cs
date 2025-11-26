using Identity.Gateways.Accounts.Responses;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Accounts.Fixtures;

namespace Tests.Identity.Accounts.Tests;

public sealed class ChangePasswordTests(AccountsTestsFixture fixture) : IClassFixture<AccountsTestsFixture>
{
    private readonly AccountFeatureFacade _facade = new(fixture.Services);
    
    [Fact]
    private async Task Change_Password_Success()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "mySeriousPassword";
        const string otherPassword = "otherPassword";
        Result<AccountResponse> account = await _facade.AddAccount(name, email, password);
        Assert.True(account.IsSuccess);
        Guid accountId = account.Value.Id;
        await _facade.MakeAccountActivated(accountId);
        Assert.True(await _facade.IsEncryptedPasswordEqualToPlain(accountId, password));
        Result<AccountResponse> changingPassword = await _facade.ChangePassword(accountId, otherPassword);
        Assert.True(changingPassword.IsSuccess);
        Assert.True(await _facade.IsEncryptedPasswordEqualToPlain(accountId, otherPassword));
    }

    [Fact]
    private async Task Change_Password_Account_Not_Activated_Failure()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "mySeriousPassword";
        const string otherPassword = "otherPassword";
        Result<AccountResponse> account = await _facade.AddAccount(name, email, password);
        Assert.True(account.IsSuccess);
        Guid accountId = account.Value.Id;
        Result<AccountResponse> changingPassword = await _facade.ChangePassword(accountId, otherPassword);
        Assert.True(changingPassword.IsFailure);
    }

    [Fact]
    private async Task Change_Account_Password_Invalid_Failure()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "mySeriousPassword";
        const string otherPassword = "   ";
        Result<AccountResponse> account = await _facade.AddAccount(name, email, password);
        Assert.True(account.IsSuccess);
        Guid accountId = account.Value.Id;
        Result<AccountResponse> changingPassword = await _facade.ChangePassword(accountId, otherPassword);
        Assert.True(changingPassword.IsFailure);
    }
    
    [Fact]
    private async Task Change_Password_Account_Not_Found_Failure()
    {
        const string otherPassword = "otherPassword";
        Result<AccountResponse> changingPassword = await _facade.ChangePassword(Guid.NewGuid(), otherPassword);
        Assert.True(changingPassword.IsFailure);
    }
}