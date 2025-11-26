using Identity.Gateways.Accounts.RequirePasswordReset;
using Identity.Gateways.Accounts.Responses;
using Identity.Gateways.AccountTickets.OnAccountTicketPasswordResetRequired;
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
        const string type = AddAccountTicketOnAccountPasswordResetRequired.Type;
        Result<AccountResponse> result = await _facade.AddAccount(name, email, password);
        Assert.True(result.IsSuccess);
        Guid accountId = result.Value.Id;
        await _facade.MakeAccountActivated(accountId);
        Result<RequirePasswordResetResponse> requiring = await _facade.RequirePasswordReset(accountId);
        Assert.True(requiring.IsSuccess);
        bool hasOutboxMessage = await _facade.HasOutboxMessageWithType(type);
        Assert.True(hasOutboxMessage);
        await Task.Delay(TimeSpan.FromSeconds(10));
        bool hasOutboxMessageProcessed = await _facade.IsOutboxMessageWithTypeProcessed(type);
        Assert.True(hasOutboxMessageProcessed);
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