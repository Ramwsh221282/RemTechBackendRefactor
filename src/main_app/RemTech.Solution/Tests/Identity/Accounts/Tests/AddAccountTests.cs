using Identity.Gateways.Accounts.Responses;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Accounts.Fixtures;

namespace Tests.Identity.Accounts.Tests;

public sealed class AddAccountTests(AccountsTestsFixture fixture) : IClassFixture<AccountsTestsFixture>
{
    private readonly AccountFeatureFacade _facade = new(fixture.Services);
    private const string email = "testEmail@gmail.com";
    private const string password = "testAccountPassword";
    private const string name = "testAccountName";

    [Fact]
    private async Task Add_Account_Success()
    {
        Result<AccountResponse> response = await _facade.AddAccount(name, email, password);
        Assert.True(response.IsSuccess);
        bool exists = await _facade.AccountCreated(response.Value.Id);
        Assert.True(exists);
    }

    [Fact]
    private async Task Add_Account_Bad_Name_Failure()
    {
        const string badName = "";
        Result<AccountResponse> response = await _facade.AddAccount(badName, email, password);
        Assert.True(response.IsFailure);
    }

    [Fact]
    private async Task Add_Account_Bad_Email_Failure()
    {
        const string badEmail = "";
        Result<AccountResponse> response = await _facade.AddAccount(name, badEmail, password);
        Assert.True(response.IsFailure);
    }

    [Fact]
    private async Task Add_Account_Bad_Password_Failure()
    {
        const string badPassword = "";
        Result<AccountResponse> response = await _facade.AddAccount(name, email, badPassword);
        Assert.True(response.IsFailure);
    }

    [Fact]
    private async Task Add_Account_Email_Is_Not_Free_Failure()
    {
        const string otherName = "otherName";
        await _facade.AddAccount(name, email, password);
        Result<AccountResponse> response = await _facade.AddAccount(otherName, email, password);
        Assert.True(response.IsFailure);
    }

    [Fact]
    private async Task Add_Account_Name_Is_Not_Free_Failure()
    {
        const string otherEmail = "otherEmail@mail.com";
        await _facade.AddAccount(name, email, password);
        Result<AccountResponse> response = await _facade.AddAccount(name, otherEmail, password);
        Assert.True(response.IsFailure);
    }
}