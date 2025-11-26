using Identity.Gateways.Accounts.RequirePasswordReset;
using Identity.Gateways.Accounts.Responses;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using Tests.Identity.Accounts.Fixtures;

namespace Tests.Identity.Accounts.Tests;

public sealed class RequirePasswordResetTests(AccountsTestsFixture fixture) : IClassFixture<AccountsTestsFixture>
{
    private readonly AccountFeatureFacade _facade = new(fixture.Services);

    [Fact]
    private async Task Require_Password_Reset_Ticket_Success()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.MakeAccountActivated(accountId);
        Result<RequirePasswordResetResponse> requiring = await _facade.RequirePasswordReset(accountId);
        Assert.True(requiring.IsSuccess);
    }

    [Fact]
    private async Task Require_Password_Reset_Ticket_Account_Not_Activated_Failure()
    {
        const string email = "someEmail@mail.com";
        const string name = "someName";
        const string password = "somePassword";
        Result<AccountResponse> result = await _facade.AddAccount(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        Result<RequirePasswordResetResponse> requiring = await _facade.RequirePasswordReset(accountId);
        Assert.True(requiring.IsFailure);
    }

    [Fact]
    private async Task Require_Password_Reset_Ticket_Account_Not_Found()
    {
        Result<RequirePasswordResetResponse> requiring = await _facade.RequirePasswordReset(Guid.NewGuid());
        Assert.True(requiring.IsFailure);
    }
}